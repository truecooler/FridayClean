using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FridayClean.Client.Api.Exceptions
{
	public abstract class GrpcExceptionBase : ApplicationException
	{
		public Metadata Trailers { get; }

		public Status Status { get; }
		public GrpcExceptionBase(string message, Status status, Metadata trailers = null)
			: base(message)
		{
			Trailers = trailers;
			Status = status;
		}

		public GrpcExceptionBase(string message, Exception inner, Status status, Metadata trailers = null)
			: base(message, inner)
		{
			Trailers = trailers;
			Status = status;
		}

	}
}
