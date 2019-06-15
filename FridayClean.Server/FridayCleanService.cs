using FridayClean.Server.SmsService;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FridayClean.Common;
using FridayClean.Server.Repositories;
using Microsoft.EntityFrameworkCore;
using FridayClean.Server.DataBaseModels;

//using FridayCleanProtocol;

namespace FridayClean.Server
{
	public class FridayCleanService : FridayCleanCommunication.FridayCleanCommunicationBase
	{

		static Dictionary<string,int> Codes = new Dictionary<string, int>();

		private ISmsService _smsService;

		private ILogger _logger;

		private BaseRepository<User, DbContext> _usersRepository;
		public FridayCleanService(ISmsService smsService, BaseRepository<User, DbContext> usersRepository, ILogger<FridayCleanService> logger)
		{
			_smsService = smsService;
			_logger = logger;
			_usersRepository = usersRepository;
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
			_usersRepository.Add(new User(){Id=1,Name="Misha",Phone ="2222"});
			return Task.FromResult(new AuthValidateTokenResponse(){ResponseStatus = request.Token == "some_token" ? AuthValidateTokenResponseStatus.ValidToken : AuthValidateTokenResponseStatus.NotValidToken});

			//ResponseStatus = AuthSendCodeResponseStatus.Ok

		}

	}





}
