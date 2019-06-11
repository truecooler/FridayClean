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

		public static FridayCleanApiSettings Default()
		{
			return new FridayCleanApiSettings() {Host = "fridayclean.thecooler.ru", Port = 80, AccessToken = ""};
		}
	}
}
