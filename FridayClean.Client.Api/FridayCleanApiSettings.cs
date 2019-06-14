using FridayClean.Common.Helpers;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
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

			//var creds = ChannelCredentials.Create(Utils.Ssl.CreateSslClientCredentials(),
			//	CallCredentials.FromInterceptor(asyncAuthInterceptor);

			return new FridayCleanApiSettings() {Host = "fridayclean.thecooler.ru", Port = 443, AccessToken = "",
				ChannelOptions = options,
				ChannelCredentials = Utils.Ssl.CreateSslClientCredentials(),
				Interceptors = new List<Interceptor>() { new AuthInterceptor() }
			};
		}

		public static FridayCleanApiSettings DevelopmentDefault()
		{
			return new FridayCleanApiSettings() { Host = "192.168.10.10", Port = 80, AccessToken = "" };
		}
	}
}
