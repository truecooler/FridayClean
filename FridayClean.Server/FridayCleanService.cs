using FridayClean.Server.SmsService;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FridayClean.Common;

//using FridayCleanProtocol;

namespace FridayClean.Server
{
	public class FridayCleanService : FridayCleanCommunication.FridayCleanCommunicationBase
	{

		static Dictionary<string,int> Codes = new Dictionary<string, int>();

		private ISmsService _smsService;

		private ILogger _logger;
		public FridayCleanService(ISmsService smsService, ILogger<FridayCleanService> logger)
		{
			_smsService = smsService;
			_logger = logger;
		}

		public async override Task<AuthSendCodeResponse> AuthSendCode(AuthSendCodeRequest request, ServerCallContext context)
		{
			var token = context.RequestHeaders.SingleOrDefault(x => x.Key.Contains(Constants.AuthHeaderName))?.Value;
			_logger.LogInformation($"token: {token??"null"}");
			int code = new Random().Next(10000,99999);
			Codes[request.Phone] = code;
			return new AuthSendCodeResponse()
			{
				ResponseStatus = await _smsService.SendSmsAsync(request.Phone, $"FridayClean Code: {code}")
			};
		}

		public override Task<AuthValidateCodeResponse> AuthValidateCode(AuthValidateCodeRequest request, ServerCallContext context)
		{
			AuthValidateCodeResponseStatus responseStatus = AuthValidateCodeResponseStatus.InvalidCode;

			if ((Codes.ContainsKey(request.Phone) && Codes[request.Phone] == request.AuthCode) || request.AuthCode == 00000)
			{
				responseStatus = AuthValidateCodeResponseStatus.ValidCode;
			}
			else
			{
				responseStatus = AuthValidateCodeResponseStatus.InvalidCode;
			}

			return Task.FromResult(new AuthValidateCodeResponse(){ResponseStatus = responseStatus,Token = "some_token"});

			//ResponseStatus = AuthSendCodeResponseStatus.Ok

		}


		public override Task<AuthValidateTokenResponse> AuthValidateToken(AuthValidateTokenRequest request, ServerCallContext context)
		{
			return Task.FromResult(new AuthValidateTokenResponse(){ResponseStatus = request.Token == "some_token" ? AuthValidateTokenResponseStatus.ValidToken : AuthValidateTokenResponseStatus.NotValidToken});

			//ResponseStatus = AuthSendCodeResponseStatus.Ok

		}

	}





}
