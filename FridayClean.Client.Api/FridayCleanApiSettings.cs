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

		private static FridayCleanApiSettings Default()
		{
			

			var settings = new FridayCleanApiSettings()
			{
				AccessToken = "loltoken",
			};

			Action<CallOptions> callback = (CallOptions x) =>
				x.Headers.Add(Constants.AuthHeaderName, settings.AccessToken);

			settings.Interceptors = new List<Interceptor>() {new AuthInterceptor(callback)};

			return settings;
		}

		public static FridayCleanApiSettings ProductionDefault()
		{
			var settings = Default();

			var options = new List<ChannelOption>
			{
				new ChannelOption(Grpc.Core.ChannelOptions.SslTargetNameOverride, Utils.Ssl.DefaultHostOverride)
			};

			settings.ChannelOptions = options;
			settings.ChannelCredentials = Utils.Ssl.CreateSslClientCredentials();

			settings.Host = "fridayclean.thecooler.ru";
			settings.Port = 443;
			return settings;
		}

		public static FridayCleanApiSettings DevelopmentDefault()
		{
			var settings = Default();
			settings.Host = "192.168.10.10";
			settings.Port = 80;
			return settings;
		}

	}
}

