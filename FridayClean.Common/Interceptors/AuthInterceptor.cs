using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace FridayClean.Common.Interceptors
{
	public class AuthInterceptor : Interceptor
	{
		private ILogger _logger;

		public AuthInterceptor()
		{
			//_logger = logger;
		}

		public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request,
			ClientInterceptorContext<TRequest, TResponse> context,
			BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
		{
			context.Options.Headers.Add(Constants.AuthHeaderName,"token!!");
			return continuation(request, context);
		}

		public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request,
			ClientInterceptorContext<TRequest, TResponse> context,
			AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
		{
			context.Options.Headers.Add(Constants.AuthHeaderName, "token!!");
			return continuation(request, context);
		}
	}
}
