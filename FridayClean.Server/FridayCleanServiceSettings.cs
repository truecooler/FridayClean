using FridayClean.Server.SmsService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FridayClean.Server
{
	public class FridayCleanServiceSettings
	{
		//private ILogger _logger;

		public string PostgresqlConnectionString;

		public SmsGatewayCredentials SmsGatewayCredentials = new SmsGatewayCredentials();

		public FridayCleanServiceSettings()
		{

		}

		public static string PathToFile
		{
			get
			{
				return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
					$"{MethodBase.GetCurrentMethod().DeclaringType.Name}.json");
			}
		}
		private static void CreateDefault()
		{
			var settings = new FridayCleanServiceSettings();
			File.WriteAllText(PathToFile, JsonConvert.SerializeObject(settings));
		}

		private static FridayCleanServiceSettings Load()
		{
			if (!File.Exists(FridayCleanServiceSettings.PathToFile))
			{
				return null;
			}

			return JsonConvert.DeserializeObject<FridayCleanServiceSettings>(File.ReadAllText(FridayCleanServiceSettings.PathToFile));
		}


		public static FridayCleanServiceSettings LoadOrCreateDefault(ILogger<FridayCleanServiceSettings> logger)
		{
			FridayCleanServiceSettings settings = null;

			if ((settings = FridayCleanServiceSettings.Load()) == null)
			{
				logger.LogWarning(
					$"Файл с настройками сервиса ({PathToFile}) не найден. Будет создан файл настроек по-умолчанию.");
				logger.LogWarning("Занесите в него корректные параметры запустите сервис снова");
				FridayCleanServiceSettings.CreateDefault();
				Console.ReadLine();
				Environment.Exit(0);
			}

			logger.LogInformation("Настройки загружены.");
			return settings;
		}

	}
}
