using Grpc.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using FridayClean.Client.Api.Exceptions;
using System.Reflection;
using System.Threading;
using FridayClean.Client.Api.Extensions;
using Grpc.Core.Interceptors;
using FridayClean.Common;

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

			var asyncAuthInterceptor = new AsyncAuthInterceptor( (context, metadata) =>
			{
				metadata.Add(Constants.AuthHeaderName, Settings.AccessToken);
				return Task.CompletedTask;
			});

			var channelCredentials = ChannelCredentials.Create(ChannelCredentials.Insecure,
				CallCredentials.FromInterceptor(asyncAuthInterceptor));

			
			_channel = new Channel($"{Settings.Host}:{Settings.Port}", channelCredentials);
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

		public Task<AuthSendCodeResponse> AuthSendCodeAsync(AuthSendCodeRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
		{
			return CallApiAndRethrowExceptionsAsync(async x => await x.AuthSendCodeAsync(request, headers, deadline, cancellationToken));
		}

		public AuthSendCodeResponse AuthSendCode(AuthSendCodeRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
		{
			return CallApiAndRethrowExceptions(x => x.AuthSendCode(request, headers, deadline, cancellationToken));
		}

		public Task<AuthValidateCodeResponse> AuthValidateCodeAsync(AuthValidateCodeRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
		{
			return CallApiAndRethrowExceptions(async x => await x.AuthValidateCodeAsync(request, headers, deadline, cancellationToken));
		}

		//public AuthValidateCodeResponse AuthValidateCode(AuthValidateCodeRequest request, CallOptions options)
		//{
		//	return CallApiAndRethrowExceptions(x => x.AuthValidateCode(request,options.Headers,options.Deadline,options.CancellationToken));
		//}

		public AuthValidateCodeResponse AuthValidateCode(AuthValidateCodeRequest request, Metadata headers = null,DateTime? deadline=null, CancellationToken cancellationToken=default)
		{
			return CallApiAndRethrowExceptions(x => x.AuthValidateCode(request, headers, deadline, cancellationToken));
		}


		public Task<AuthValidateTokenResponse> AuthValidateTokenAsync(AuthValidateTokenRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
		{
			return CallApiAndRethrowExceptions(async x => await x.AuthValidateTokenAsync(request, headers, deadline, cancellationToken));
		}

		public AuthValidateTokenResponse AuthValidateToken(AuthValidateTokenRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
		{
			return CallApiAndRethrowExceptions( x=>x.AuthValidateToken(request, headers, deadline, cancellationToken));
		}

		public void Dispose()
		{
			_channel.ShutdownAsync().Wait();
		}
	}
}
