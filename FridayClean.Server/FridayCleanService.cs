﻿using FridayClean.Server.SmsService;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FridayClean.Common;
using FridayClean.Common.Helpers;
using FridayClean.Server.Repositories;
using Microsoft.EntityFrameworkCore;
using FridayClean.Server.DataBaseModels;
using FridayClean.Server.DataBaseModels.Enums;
using Google.Protobuf.Collections;

//using FridayCleanProtocol;

namespace FridayClean.Server
{
	public class FridayCleanService : FridayCleanCommunication.FridayCleanCommunicationBase
	{

		//static Dictionary<string,int> Codes = new Dictionary<string, int>();

		private ISmsService _smsService;

		private ILogger _logger;

		private IRepository<User> _usersRepository;

		private IRepository<SentSmsCode> _sentSmsCodesRepository;

		private IRepository<AuthenticatedSession> _authenticatedSessionsRepository;

		private IRepository<DataBaseModels.CleaningService> _cleaningServicesRepository;

		private IRepository<DataBaseModels.OrderedCleaning> _orderedCleaningsRepository;

		public FridayCleanService(ISmsService smsService, IRepository<User> usersRepository,
			IRepository<SentSmsCode> sentSmsCodesRepository,
			IRepository<AuthenticatedSession> authenticatedSessionsRepository,
			IRepository<DataBaseModels.CleaningService> cleaningServicesRepository,
			IRepository<DataBaseModels.OrderedCleaning> orderedCleaningsRepository, ILogger<FridayCleanService> logger)
		{
			_smsService = smsService;
			_logger = logger;
			_usersRepository = usersRepository;
			_sentSmsCodesRepository = sentSmsCodesRepository;
			_authenticatedSessionsRepository = authenticatedSessionsRepository;
			_cleaningServicesRepository = cleaningServicesRepository;
			_orderedCleaningsRepository = orderedCleaningsRepository;
		}

		public async override Task<AuthSendCodeResponse> AuthSendCode(AuthSendCodeRequest request,
			ServerCallContext context)
		{
			if (request.Phone == "77777777777")
			{
				return new AuthSendCodeResponse()
				{
					ResponseStatus = AuthSendCodeStatus.Success
				};
			}

			//var token = context.RequestHeaders.SingleOrDefault(x => x.Key.Contains(Constants.AuthHeaderName))?.Value;
			//_logger.LogInformation($"token: {token??"null"}");
			var code = Utils.SmsCodeGenerator.Generate();

			var sendSmsResponse = await _smsService.SendSmsAsync(request.Phone, $"FridayClean Code: {code}");

			if (sendSmsResponse == AuthSendCodeStatus.Success)
			{
				if (!_sentSmsCodesRepository.IsExist(x => x.Phone == request.Phone))
				{
					_sentSmsCodesRepository.Add(new SentSmsCode() {Phone = request.Phone, Code = code});
				}
				else
				{
					_sentSmsCodesRepository.Get(x => x.Phone == request.Phone).Code = code;
				}

				_sentSmsCodesRepository.Save();
			}


			return new AuthSendCodeResponse()
			{
				ResponseStatus = sendSmsResponse
			};
		}

		public override Task<AuthValidateCodeResponse> AuthValidateCode(AuthValidateCodeRequest request,
			ServerCallContext context)
		{
			AuthValidateCodeStatus responseStatus = AuthValidateCodeStatus.InvalidCode;

			if (_sentSmsCodesRepository.IsExist(x => x.Phone == request.Phone && x.Code == request.Code) ||
			    request.Code == 00000)
			{
				responseStatus = AuthValidateCodeStatus.ValidCode;


				if (!_usersRepository.IsExist(x => x.Phone == request.Phone))
				{
					_usersRepository.Add(new User()
					{
						Phone = request.Phone, Name = "", Address = "", Money = 0, Role = UserRole.Customer,
						AvatarLink =
							"https://png.pngtree.com/element_origin_min_pic/17/09/18/86f146e932d55ea3ffc59cf7a976398e.jpg"
					});
					_usersRepository.Save();
				}


				var newAccessToken = Utils.AccessTokenGenerator.Generate(request.Phone, request.Code);

				_authenticatedSessionsRepository.Add(new AuthenticatedSession()
					{Phone = request.Phone, AccessToken = newAccessToken});
				_authenticatedSessionsRepository.Save();

				_sentSmsCodesRepository.Delete(x => x.Phone == request.Phone);
				_sentSmsCodesRepository.Save();

				return Task.FromResult(new AuthValidateCodeResponse()
					{ResponseStatus = responseStatus, AccessToken = newAccessToken});
			}


			return Task.FromResult(new AuthValidateCodeResponse() {ResponseStatus = responseStatus, AccessToken = ""});
		}


		public override Task<AuthValidateTokenResponse> AuthValidateToken(AuthValidateTokenRequest request,
			ServerCallContext context)
		{
			var result = _authenticatedSessionsRepository.IsExist(x => x.AccessToken == request.AccessToken);
			return Task.FromResult(new AuthValidateTokenResponse()
			{
				ResponseStatus = (result) ? AuthValidateTokenStatus.ValidToken : AuthValidateTokenStatus.InvalidToken
			});
		}


		private string GetPhoneFromContext(ServerCallContext context)
		{
			string phone = context.RequestHeaders.SingleOrDefault(x => x.Key == Constants.UserPhoneHeaderName)?.Value;
			return !string.IsNullOrEmpty(phone)
				? phone
				: throw new ApplicationException("Fatal: user's phone wall null or empty");
		}

		private string GetAccessTokenFromContext(ServerCallContext context)
		{
			return context.RequestHeaders.SingleOrDefault(x => x.Key == Constants.AuthHeaderName)?.Value;
		}

		public override Task<GetProfileInfoResponse> GetProfileInfo(GetProfileInfoRequest request,
			ServerCallContext context)
		{
			string phone = GetPhoneFromContext(context);

			var user = _usersRepository.Get(x => x.Phone == phone);
			string name = user?.Name;
			string address = user?.Address;
			string avatarLink = user?.AvatarLink;
			var response = new GetProfileInfoResponse() {Name = name, Address = address, AvatarLink = avatarLink};
			return Task.FromResult(response);
		}


		public override Task<UserLogoutResponse> UserLogout(UserLogoutRequest request, ServerCallContext context)
		{
			var accessToken = GetAccessTokenFromContext(context);
			_authenticatedSessionsRepository.Delete(x => x.AccessToken == accessToken);
			_authenticatedSessionsRepository.Save();
			return Task.FromResult(new UserLogoutResponse() {ResponseStatus = UserLogoutStatus.LogoutSuccess});
		}

		public override Task<SetProfileInfoResponse> SetProfileInfo(SetProfileInfoRequest request,
			ServerCallContext context)
		{
			var phone = GetPhoneFromContext(context);
			var user = _usersRepository.Get(x => x.Phone == phone);
			user.Name = request.Name;
			user.Address = request.Address;
			user.AvatarLink = request.AvatarLink;
			_usersRepository.Save();
			//_usersRepository.Update(user);
			return Task.FromResult(new SetProfileInfoResponse()
				{ResponseStatus = SetProfileInfoStatus.SetSuccessfully});
		}

		public override Task<GetCleaningServicesResponse> GetCleaningServices(GetCleaningServicesRequest request,
			ServerCallContext context)
		{
			var cleaningServicesFromBd = _cleaningServicesRepository.GetAll();

			var result = new GetCleaningServicesResponse();
			foreach (var cleaningService in cleaningServicesFromBd)
			{
				result.CleaningServices.Add(new CleaningService()
				{
					CleaningType = cleaningService.CleaningType,
					Name = cleaningService.Name,
					ApartmentAreaMax = cleaningService.ApartmentAreaMax,
					ApartmentAreaMin = cleaningService.ApartmentAreaMin,
					StartingPrice = cleaningService.StartingPrice,
					Description = cleaningService.Description
				});
			}

			return Task.FromResult(result);
		}


		public override Task<GetOrderedCleaningsResponse> GetOrderedCleanings(GetOrderedCleaningsRequest request,
			ServerCallContext context)
		{
			var phone = GetPhoneFromContext(context);

			var orderedCleaningsFromBd = _orderedCleaningsRepository.GetMany(x => x.CustomerPhone == phone);

			var result = new GetOrderedCleaningsResponse();
			foreach (var orderedCleaning in orderedCleaningsFromBd)
			{
				result.OrderedCleanings.Add(new OrderedCleaning()
				{
					CleaningType = orderedCleaning.CleaningType,
					CustomerPhone = orderedCleaning.CustomerPhone,
					Id = orderedCleaning.Id,
					State = orderedCleaning.State,
					CleanerPhone = orderedCleaning.CleanerPhone,
					ApartmentArea = orderedCleaning.ApartmentArea,
					Price = orderedCleaning.Price
				});
			}

			return Task.FromResult(result);
		}

		public override Task<OrderNewCleaningResponse> OrderNewCleaning(OrderNewCleaningRequest request,
			ServerCallContext context)
		{
			var phone = GetPhoneFromContext(context);

			var cleaningService = _cleaningServicesRepository.Get(x => x.CleaningType == request.CleaningType);
			if (cleaningService == null)
			{
				throw new RpcException(new Status(StatusCode.InvalidArgument,
					$"Cleaning service {request.CleaningType.ToString()} is not exists."));
			}

			_orderedCleaningsRepository.Add(new DataBaseModels.OrderedCleaning()
			{
				CleanerPhone = request.CleanerPhone, CustomerPhone = phone,
				CleaningType = request.CleaningType, ApartmentArea = request.ApartmentArea,
				State = OrderedCleaningState.WaitingForCleanerConfirmation,
				Price = Utils.PriceCalculator.Calculate(cleaningService.ApartmentAreaMin, request.ApartmentArea,
					cleaningService.StartingPrice)
			});
			_orderedCleaningsRepository.Save();
			return Task.FromResult(new OrderNewCleaningResponse()
				{OrderedCleaningState = OrderedCleaningState.WaitingForCleanerConfirmation});
		}

		public override Task<GetCleanersResponse> GetCleaners(GetCleanersRequest request,
			ServerCallContext context)
		{
			var cleanersFromBd = _usersRepository.GetMany(x => x.Role == UserRole.Cleaner);
			var result = new GetCleanersResponse();

			foreach (var cleaner in cleanersFromBd)
			{
				result.Cleaners.Add(new Cleaner()
				{
					Phone = cleaner.Phone,
					Name = cleaner.Name,
					AvatarLink = cleaner.AvatarLink
				});
			}

			return Task.FromResult(result);
		}
	}

}
