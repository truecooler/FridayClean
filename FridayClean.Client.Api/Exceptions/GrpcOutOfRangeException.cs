using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FridayClean.Client.Api.Exceptions
{
	public class GrpcOutOfRangeException : GrpcExceptionBase
	{
		public GrpcOutOfRangeException(string message, Status status, Metadata trailers = null)
			: base(message, status, trailers)
		{
		}

		public GrpcOutOfRangeException(string message, Exception inner, Status status, Metadata trailers = null)
			: base(message, inner, status, trailers)
		{
		}
	}
}
