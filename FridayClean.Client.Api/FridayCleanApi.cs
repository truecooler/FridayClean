using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace FridayClean.Client.Api
{
	
	public class FridayCleanApi : IFridayCleanApi
	{
		private Channel _channel;

		private FridayCleanCommunication.FridayCleanCommunicationClient _client;

		public readonly FridayCleanApiSettings Settings;

		public FridayCleanApi(FridayCleanApiSettings settings)
		{
			Settings = settings;
			_channel = new Channel($"{Settings.Host}:{Settings.Port}", SslCredentials.Insecure);
			_client = new FridayCleanCommunication.FridayCleanCommunicationClient(_channel);
		}

		public async Task<AuthSendCodeResponse> AuthSendCodeAsync(AuthSendCodeRequest request)
		{
			return await _client.AuthSendCodeAsync(request);
		}

		public AuthSendCodeResponse AuthSendCode(AuthSendCodeRequest request)
		{
			return _client.AuthSendCode(request);
		}

		public async Task<AuthValidateCodeResponse> AuthValidateCodeAsync(AuthValidateCodeRequest request)
		{
			return await _client.AuthValidateCodeAsync(request);
		}

		public AuthValidateCodeResponse AuthValidateCode(AuthValidateCodeRequest request)
		{
			return _client.AuthValidateCode(request);
		}


		public async Task<AuthValidateTokenResponse> AuthValidateTokenAsync(AuthValidateTokenRequest request)
		{
			return await _client.AuthValidateTokenAsync(request);
		}

		public AuthValidateTokenResponse AuthValidateToken(AuthValidateTokenRequest request)
		{
			return _client.AuthValidateToken(request);
		}

		public void Dispose()
		{
			_channel.ShutdownAsync().Wait();
		}
	}
}
