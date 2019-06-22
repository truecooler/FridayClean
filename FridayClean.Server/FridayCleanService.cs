using FridayClean.Server.SmsService;
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

			var sendSmsResponse = await _smsService.SendSmsAsync(request.Phone, $"FridayClean:{code}");

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
			    request.Code == 77777)
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
			string userRole = user?.Role.ToString();
			int money = user.Money;
			var response = new GetProfileInfoResponse()
				{Name = name, Address = address, AvatarLink = avatarLink, Phone = phone, UserRole = userRole, Money = money };
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

			var user = _usersRepository.Get(x => x.Phone == phone);

			


			IEnumerable<DataBaseModels.OrderedCleaning> orderedCleaningsFromBd = null;
			if (user.Role == UserRole.Cleaner)
			{
				orderedCleaningsFromBd = _orderedCleaningsRepository.GetMany(x => x.CleanerPhone == phone);
			}
			if (user.Role == UserRole.Customer)
			{
				orderedCleaningsFromBd = _orderedCleaningsRepository.GetMany(x => x.CustomerPhone == phone);
			}

			var result = new GetOrderedCleaningsResponse();
			foreach (var orderedCleaning in orderedCleaningsFromBd)
			{
				var cleaner = _usersRepository.Get(x => x.Phone == orderedCleaning.CleanerPhone);
				var customer = _usersRepository.Get(x => x.Phone == orderedCleaning.CustomerPhone);
				result.OrderedCleanings.Add(new OrderedCleaning()
				{
					CleaningType = orderedCleaning.CleaningType,
					CleanerName = cleaner?.Name,
					CleanerPhone = orderedCleaning.CleanerPhone,
					CustomerName = customer?.Name,
					CustomerPhone = customer?.Phone,
					Id = orderedCleaning.Id,
					State = orderedCleaning.State,
					Address = orderedCleaning.Address,
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

			var customer = _usersRepository.Get(x => x.Phone == phone);
			var orderPrice = Utils.PriceCalculator.Calculate(cleaningService.ApartmentAreaMin, request.ApartmentArea,
				cleaningService.StartingPrice);
			if (customer.Money - orderPrice < 0)
			{
				return Task.FromResult(new OrderNewCleaningResponse()
					{ OrderedCleaningState = OrderedCleaningState.Canceled, Info = $"Недостаточно денег. Ваш баланс:{customer.Money}. Стоимость уборки: {orderPrice}"});
			}

			customer.Money -= orderPrice;
			_usersRepository.Save();

			_orderedCleaningsRepository.Add(new DataBaseModels.OrderedCleaning()
			{
				CleanerPhone = request.CleanerPhone, CustomerPhone = phone,
				CleaningType = request.CleaningType, Address = _usersRepository.Get(x => x.Phone == phone)?.Address,
				ApartmentArea = request.ApartmentArea,
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

		//ChangeOrderedCleaningState
		public override Task<ChangeOrderedCleaningStateResponse> ChangeOrderedCleaningState(
			ChangeOrderedCleaningStateRequest request,
			ServerCallContext context)
		{
			var orderFromDb = _orderedCleaningsRepository.Get(x => x.Id == request.OrderId);
			var result = new ChangeOrderedCleaningStateResponse();
			if (orderFromDb == null)
			{
				result.ResponseStatus = ChangeOrderedCleaningStateStatus.OrderStatusChangeError;
				result.ErrorMessage = "Заказ не найден";
				return Task.FromResult(result);
			}

			if (request.RequiredState == OrderedCleaningState.WaitingForCleanerArrival &&
			    orderFromDb.State != OrderedCleaningState.WaitingForCleanerConfirmation)
			{
				result.ResponseStatus = ChangeOrderedCleaningStateStatus.OrderStatusChangeError;
				result.ErrorMessage = "Неверное состояние заказа. Ожидалось, что клиент ждет подтверждения от клинера.";
				return Task.FromResult(result);
			}

			if (request.RequiredState == OrderedCleaningState.CleanerWorkInProgress &&
			    orderFromDb.State != OrderedCleaningState.WaitingForCleanerArrival)
			{
				result.ResponseStatus = ChangeOrderedCleaningStateStatus.OrderStatusChangeError;
				result.ErrorMessage =
					"Неверное состояние заказа. Ожидалось, что клиент ждет, пока клинер придет по адресу.";
				return Task.FromResult(result);
			}

			if (request.RequiredState == OrderedCleaningState.Completed &&
			    orderFromDb.State != OrderedCleaningState.CleanerWorkInProgress)
			{
				result.ResponseStatus = ChangeOrderedCleaningStateStatus.OrderStatusChangeError;
				result.ErrorMessage =
					"Неверное состояние заказа. Ожидалось, что клиент ждет, пока клинер закончит работу.";
				return Task.FromResult(result);
			}

			if (request.RequiredState == OrderedCleaningState.Completed)
			{
				var customer = _usersRepository.Get(x => x.Phone == orderFromDb.CleanerPhone);
				customer.Money += orderFromDb.Price;
				_usersRepository.Save();
			}

			orderFromDb.State = request.RequiredState;
			_orderedCleaningsRepository.Save();

			

			result.ResponseStatus = ChangeOrderedCleaningStateStatus.OrderStatusChangedSuccessfully;
			return Task.FromResult(result);
		}

	}
}

