using System;
using System.Collections.Generic;
using System.Text;

namespace FridayClean.Client.Api
{
	public class FridayCleanApiSettings
	{
		public string Host;
		public int Port;
		public string AccessToken;

		public static FridayCleanApiSettings ProductionDefault()
		{
			return new FridayCleanApiSettings() {Host = "fridayclean.thecooler.ru", Port = 443, AccessToken = ""};
		}

		public static FridayCleanApiSettings DevelopmentDefault()
		{
			return new FridayCleanApiSettings() { Host = "192.168.10.10", Port = 80, AccessToken = "" };
		}
	}
}
