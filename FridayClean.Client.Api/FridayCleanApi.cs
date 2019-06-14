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
using System.Runtime.ExceptionServices;
using Grpc.Core.Utils;
using FridayClean.Common.Helpers;
using System.Collections.Generic;
using System.Diagnostics;

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

			//var asyncAuthInterceptor = new AsyncAuthInterceptor( (context, metadata) =>
			//{
			//	metadata.Add(Constants.AuthHeaderName, Settings.AccessToken);
			//	return Task.CompletedTask;
			//});

			//var options = new List<ChannelOption>
			//{
			//	//move ssl creds to ioc
			//	new ChannelOption(ChannelOptions.SslTargetNameOverride, Utils.Ssl.DefaultHostOverride)
			//};

			//var channelCredentials = ChannelCredentials.Create(Utils.Ssl.CreateSslClientCredentials(),
			//	CallCredentials.FromInterceptor(asyncAuthInterceptor));

			
			_channel = new Channel($"{Settings.Host}:{Settings.Port}", Settings.ChannelCredentials, Settings.ChannelOptions);
			var interceptedChannel = _channel.Intercept(Settings.Interceptors.ToArray());
			_client = new FridayCleanCommunication.FridayCleanCommunicationClient(interceptedChannel);
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

			throw (GrpcExceptionBase)Activator.CreateInstance(type, ex.Message, ex, ex.Status,
				ex.Trailers);
		}


		private Task<TResult> CallApiAndRethrowExceptionsAsync<TResult>(
			Func<FridayCleanCommunication.FridayCleanCommunicationClient, Task<TResult>> func)
		{

			return func(_client).ContinueWith( x=>
			{
				if (x.IsFaulted)
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
				}

				return x.Result;

			});
			
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

		/* new metadata creating needed to add header while intercepting.
		 we are unable to create metadata by intercepter, because header property is readonly */

		public  Task<AuthSendCodeResponse> AuthSendCodeAsync(AuthSendCodeRequest request)
		{
			//return await _client.AuthSendCodeAsync(request, headers, deadline, cancellationToken);
			return CallApiAndRethrowExceptionsAsync(async x => await x.AuthSendCodeAsync(request,new Metadata()));
		}

		public AuthSendCodeResponse AuthSendCode(AuthSendCodeRequest request)
		{
			return CallApiAndRethrowExceptions(x => x.AuthSendCode(request));
		}

		public Task<AuthValidateCodeResponse> AuthValidateCodeAsync(AuthValidateCodeRequest request)
		{
			return CallApiAndRethrowExceptions(async x => await x.AuthValidateCodeAsync(request, new Metadata()));
		}


		public AuthValidateCodeResponse AuthValidateCode(AuthValidateCodeRequest request)
		{
			return CallApiAndRethrowExceptions(x => x.AuthValidateCode(request, new Metadata()));
		}


		public Task<AuthValidateTokenResponse> AuthValidateTokenAsync(AuthValidateTokenRequest request)
		{
			return CallApiAndRethrowExceptions(async x => await x.AuthValidateTokenAsync(request, new Metadata()));
		}

		public AuthValidateTokenResponse AuthValidateToken(AuthValidateTokenRequest request)
		{
			return CallApiAndRethrowExceptions( x=>x.AuthValidateToken(request, new Metadata()));
		}

		public void Dispose()
		{
			_channel.ShutdownAsync().Wait();
		}
	}
}
