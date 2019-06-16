using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace FridayClean.Common.Interceptors
{
	public class AuthInterceptor : Interceptor
	{
		//private ILogger _logger;


		private Action<CallOptions> _clientCallback;
		private Action<ServerCallContext> _serverCallback;
		public AuthInterceptor(Action<CallOptions> clientCallback)
		{
			_clientCallback = clientCallback;
		}

		public AuthInterceptor(Action<ServerCallContext> serverCallback)
		{
			_serverCallback = serverCallback;
		}

		public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request,
			ClientInterceptorContext<TRequest, TResponse> context,
			BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
		{
			_clientCallback(context.Options);
			return continuation(request, context);
		}

		public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request,
			ClientInterceptorContext<TRequest, TResponse> context,
			AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
		{
			_clientCallback(context.Options);
			return continuation(request, context);
		}

		public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
			ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
		{
			_serverCallback(context);
			return continuation(request, context);
		}
	}
}
