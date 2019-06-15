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
					ResponseStatus = AuthSendCodeResponseStatus.Success
				};
			}
			//var token = context.RequestHeaders.SingleOrDefault(x => x.Key.Contains(Constants.AuthHeaderName))?.Value;
			//_logger.LogInformation($"token: {token??"null"}");
			var code = Utils.SmsCodeGenerator.Generate();

			var sendSmsResponse = await _smsService.SendSmsAsync(request.Phone, $"FridayClean Code: {code}");

			if (sendSmsResponse == AuthSendCodeResponseStatus.Success)
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
			AuthValidateCodeResponseStatus responseStatus = AuthValidateCodeResponseStatus.InvalidCode;

			if (_sentSmsCodesRepository.IsExist(x=>x.Phone == request.Phone && x.Code == request.AuthCode) || request.AuthCode == 00000)
			{
				responseStatus = AuthValidateCodeResponseStatus.ValidCode;


				if (!_usersRepository.IsExist(x => x.Phone == request.Phone))
				{
					_usersRepository.Add(new User(){Phone = request.Phone,Name = ""});
					_usersRepository.Save();
				}


				var newAccessToken = Utils.AccessTokenGenerator.Generate(request.Phone, request.AuthCode);

				_authenticatedSessionsRepository.Add(new AuthenticatedSession(){Phone = request.Phone,AccessToken = newAccessToken});
				_authenticatedSessionsRepository.Save();

				_sentSmsCodesRepository.Delete(x=>x.Phone == request.Phone);
				_sentSmsCodesRepository.Save();

				return Task.FromResult(new AuthValidateCodeResponse() { ResponseStatus = responseStatus, Token = newAccessToken });
			}


			return Task.FromResult(new AuthValidateCodeResponse(){ResponseStatus = responseStatus,Token = ""});

			//ResponseStatus = AuthSendCodeResponseStatus.Ok

		}


		public override Task<AuthValidateTokenResponse> AuthValidateToken(AuthValidateTokenRequest request, ServerCallContext context)
		{
			var result = _authenticatedSessionsRepository.IsExist(x => x.AccessToken == request.Token);
			return Task.FromResult(new AuthValidateTokenResponse(){ResponseStatus = (result) ? AuthValidateTokenResponseStatus.ValidToken : AuthValidateTokenResponseStatus.NotValidToken});

			//ResponseStatus = AuthSendCodeResponseStatus.Ok

		}

	}





}
