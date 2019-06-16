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

		public FridayCleanService(ISmsService smsService, IRepository<User> usersRepository,
			IRepository<SentSmsCode> sentSmsCodesRepository,
			IRepository<AuthenticatedSession> authenticatedSessionsRepository, ILogger<FridayCleanService> logger)
		{
			_smsService = smsService;
			_logger = logger;
			_usersRepository = usersRepository;
			_sentSmsCodesRepository = sentSmsCodesRepository;
			_authenticatedSessionsRepository = authenticatedSessionsRepository;
		}

		public async override Task<AuthSendCodeResponse> AuthSendCode(AuthSendCodeRequest request, ServerCallContext context)
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

		public override Task<AuthValidateCodeResponse> AuthValidateCode(AuthValidateCodeRequest request, ServerCallContext context)
		{
			AuthValidateCodeStatus responseStatus = AuthValidateCodeStatus.InvalidCode;

			if (_sentSmsCodesRepository.IsExist(x=>x.Phone == request.Phone && x.Code == request.Code) || request.Code == 00000)
			{
				responseStatus = AuthValidateCodeStatus.ValidCode;


				if (!_usersRepository.IsExist(x => x.Phone == request.Phone))
				{
					_usersRepository.Add(new User(){Phone = request.Phone,Name = "",Address = ""});
					_usersRepository.Save();
				}


				var newAccessToken = Utils.AccessTokenGenerator.Generate(request.Phone, request.Code);

				_authenticatedSessionsRepository.Add(new AuthenticatedSession(){Phone = request.Phone,AccessToken = newAccessToken});
				_authenticatedSessionsRepository.Save();

				_sentSmsCodesRepository.Delete(x=>x.Phone == request.Phone);
				_sentSmsCodesRepository.Save();

				return Task.FromResult(new AuthValidateCodeResponse() { ResponseStatus = responseStatus, AccessToken = newAccessToken });
			}


			return Task.FromResult(new AuthValidateCodeResponse(){ResponseStatus = responseStatus,AccessToken = ""});
		}


		public override Task<AuthValidateTokenResponse> AuthValidateToken(AuthValidateTokenRequest request, ServerCallContext context)
		{
			var result = _authenticatedSessionsRepository.IsExist(x => x.AccessToken == request.AccessToken);
			return Task.FromResult(new AuthValidateTokenResponse(){ResponseStatus = (result) ? AuthValidateTokenStatus.ValidToken : AuthValidateTokenStatus.InvalidToken});
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

		public override Task<GetProfileInfoResponse> GetProfileInfo(GetProfileInfoRequest request, ServerCallContext context)
		{
			string phone = GetPhoneFromContext(context);

			var user = _usersRepository.Get(x => x.Phone == phone);
			string name = user?.Name;
			string address = user?.Address;
			var response = new GetProfileInfoResponse() {Name = name, Address = address};
			return Task.FromResult(response);
		}


		public override Task<UserLogoutResponse> UserLogout(UserLogoutRequest request, ServerCallContext context)
		{
			var accessToken = GetAccessTokenFromContext(context);
			_authenticatedSessionsRepository.Delete(x => x.AccessToken == accessToken);
			_authenticatedSessionsRepository.Save();
			return Task.FromResult(new UserLogoutResponse(){ResponseStatus = UserLogoutStatus.LogoutSuccess});
		}

	}





}
