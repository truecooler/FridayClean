﻿using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FridayClean.Server.SmsService
{
	public class SmscSmsService : ISmsService
	{
		private ILogger _logger;

		private SmsGatewayCredentials _credentials;
		private class SmscSmsServiceResponse
		{
			public int Id { get; set; }
			public int Cnt { get; set; }
			public string Cost { get; set; }
			public string Balance { get; set; }

			public string Error { get; set; }

			[DeserializeAs(Name = "error_code")]
			public int ErrorCode { get; set; }
		}

		private enum SmsGatewayErrorCodes
		{
			NotAnError = 0,
			InvalidPhone = 7
		}

		private IRestClient _restClient;
		public SmscSmsService(IRestClient restClient, ILogger<SmscSmsService> logger, FridayCleanServiceSettings settings)
		{
			_credentials = settings.SmsGatewayCredentials;
			_logger = logger;
			_restClient = restClient;
			_restClient.BaseUrl = new Uri("https://smsc.ru/");

		}
		public AuthSendCodeStatus SendSms(string number, string message)
		{
			return this.SendSmsAsync(number,message).GetAwaiter().GetResult();
		}

		public async Task<AuthSendCodeStatus> SendSmsAsync(string number, string message)
		{
			
			var request = new RestRequest("sys/send.php", Method.GET);
			request.RequestFormat = DataFormat.Json;

			request.AddParameter("login", _credentials.Login);
			request.AddParameter("psw", _credentials.Password);
			request.AddParameter("phones", number);
			request.AddParameter("mes", message);
			request.AddParameter("cost", 3);
			request.AddParameter("fmt", 3);

			var response = await _restClient.ExecuteTaskAsync<SmscSmsServiceResponse>(request);

			if (response.ResponseStatus != ResponseStatus.Completed)
			{
				return AuthSendCodeStatus.GateWayError;
			}

			if (response.StatusCode != HttpStatusCode.OK)
			{
				return AuthSendCodeStatus.GateWayError;
			}

			SmscSmsServiceResponse json = response.Data;

			_logger.LogInformation($"SmsGatewayResponse: {Environment.NewLine}{response.Content}");

			if (json.ErrorCode == (int)SmsGatewayErrorCodes.InvalidPhone)
			{
				return AuthSendCodeStatus.InvalidPhone;
			}

			if (json.ErrorCode != (int)SmsGatewayErrorCodes.NotAnError)
			{
				return AuthSendCodeStatus.GateWayError;
			}

			return AuthSendCodeStatus.Success;
		}
	}
}
