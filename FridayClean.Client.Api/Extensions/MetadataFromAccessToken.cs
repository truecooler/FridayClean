using System;
using System.Collections.Generic;
using System.Text;
using FridayClean.Common;
using Grpc.Core;

namespace FridayClean.Client.Api.Extensions
{
	public static class MetadataFromAccessToken
	{
		public static Metadata FromAccessToken(this Metadata headers, string accessToken)
		{
			var result = new Metadata();
			result.Add(Constants.AuthHeaderName, accessToken);
			return result;
		}
	}
}
