using FridayClean.Common.Helpers;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FridayClean.Common;
using FridayClean.Common.Interceptors;
using Grpc.Core.Interceptors;

namespace FridayClean.Client.Api
{
	public class FridayCleanApiSettings
	{
		public string Host;
		public int Port;
		public string AccessToken;
		public ChannelCredentials ChannelCredentials;
		public List<ChannelOption> ChannelOptions;
		public List<Interceptor> Interceptors;

		public FridayCleanApiSettings()
		{
			ChannelCredentials = ChannelCredentials.Insecure;
			ChannelOptions = new List<ChannelOption>();
			Interceptors = new List<Interceptor>();
		}
		public static FridayCleanApiSettings ProductionDefault()
		{
			var options = new List<ChannelOption>
			{
				//move ssl creds to ioc
				new ChannelOption(Grpc.Core.ChannelOptions.SslTargetNameOverride, Utils.Ssl.DefaultHostOverride)
			};

			var result = new FridayCleanApiSettings()
			{
				Host = "fridayclean.thecooler.ru",
				Port = 443,
				AccessToken = "loltoken",
				ChannelOptions = options,
				ChannelCredentials = Utils.Ssl.CreateSslClientCredentials()
			};

			Action<CallOptions> callback = (CallOptions x) => x.Headers.Add(Constants.AuthHeaderName, result.AccessToken);

			result.Interceptors = new List<Interceptor>() {new AuthInterceptor(callback)};

			return result;
		}

		public static FridayCleanApiSettings DevelopmentDefault()
		{
			return new FridayCleanApiSettings() { Host = "192.168.10.10", Port = 80, AccessToken = "" };
		}
	}
}
