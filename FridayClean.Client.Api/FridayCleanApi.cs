using Grpc.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using FridayClean.Client.Api.Exceptions;
using System.Reflection;

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

		private void FindExceptionTypeAndThrow(RpcException ex)
		{
			var statusCodeEnumName = Enum.GetName(typeof(StatusCode), ex.StatusCode);

			Assembly assembly = typeof(FridayCleanApi).Assembly;
			Type type = assembly.GetTypes()
				.SingleOrDefault(x => x.Name.Contains($"Grpc{statusCodeEnumName}Exception"));

			if (type == null)
			{
				throw new ApplicationException("Can't find the type of catched grpc exception");
			}

			throw (GrpcExceptionBase)Activator.CreateInstance(type, ex.Message, ex.InnerException, ex.Status,
				ex.Trailers);
		}


		private Task<TResult> CallApiAndRethrowExceptionsAsync<TResult>(
			Func<FridayCleanCommunication.FridayCleanCommunicationClient, Task<TResult>> func)
		{

			return func(_client).ContinueWith( x=>
			{
				foreach (Exception unpackedEx in x.Exception.InnerExceptions)
				{
					if (unpackedEx is RpcException rpcEx)
					{
						FindExceptionTypeAndThrow(rpcEx);
					}
					else
					{
						throw x.Exception;
					}
				}
				
				return x.Result;

			},TaskContinuationOptions.OnlyOnFaulted);
			
		}

		private TResult CallApiAndRethrowExceptions<TResult>(
			Func<FridayCleanCommunication.FridayCleanCommunicationClient, TResult> func)
		{
			try
			{
				return func(_client);
			}
			catch (RpcException ex)
			{
				FindExceptionTypeAndThrow(ex);
				return default;
			}
		}

		public  Task<AuthSendCodeResponse> AuthSendCodeAsync(AuthSendCodeRequest request)
		{
			return  CallApiAndRethrowExceptionsAsync(async x => await x.AuthSendCodeAsync(request));
		}

		public AuthSendCodeResponse AuthSendCode(AuthSendCodeRequest request)
		{
			return CallApiAndRethrowExceptions(x => x.AuthSendCode(request));
		}

		public Task<AuthValidateCodeResponse> AuthValidateCodeAsync(AuthValidateCodeRequest request)
		{
			return CallApiAndRethrowExceptions(async x => await x.AuthValidateCodeAsync(request));
		}

		public AuthValidateCodeResponse AuthValidateCode(AuthValidateCodeRequest request)
		{
			return CallApiAndRethrowExceptions(x => x.AuthValidateCode(request));
		}


		public Task<AuthValidateTokenResponse> AuthValidateTokenAsync(AuthValidateTokenRequest request)
		{
			return CallApiAndRethrowExceptions(async x => await x.AuthValidateTokenAsync(request));
		}

		public AuthValidateTokenResponse AuthValidateToken(AuthValidateTokenRequest request)
		{
			return CallApiAndRethrowExceptions( x=>x.AuthValidateToken(request));
		}

		public void Dispose()
		{
			_channel.ShutdownAsync().Wait();
		}
	}
}
