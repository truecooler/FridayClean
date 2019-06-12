using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FridayClean.Client.Helpers
{
	partial class Utils
	{
		public class AppCrashHelper
		{

			private static string CrashLogFilePath
			{
				get
				{ // iOS: Environment.SpecialFolder.Resources
					return  Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Fatal.log");
				}
			}

			public static bool IsCrashLogExists
			{
				get { return File.Exists(CrashLogFilePath); }
			}

			public static string ReadCrashLogFile(bool deleteAfterRead = true)
			{
				if (!IsCrashLogExists)
				{
					return null;
				}

				var data = File.ReadAllText(CrashLogFilePath);

				if (deleteAfterRead)
				{
					File.Delete(CrashLogFilePath);
				}

				return data;
			}

			public static void SubscribeForUnhandledAndUnobservedExceptions()
			{
				AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
				TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
			}

			private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
			{
				var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
				LogUnhandledException(newExc);
			}

			private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
			{
				var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
				LogUnhandledException(newExc);
			}

			private static void LogUnhandledException(Exception exception)
			{
				try
				{
					var errorMessage = String.Format("Time: {0}\r\nError: Unhandled Exception\r\n{1}",
						DateTime.Now, exception.ToString());
					File.WriteAllText(CrashLogFilePath, errorMessage);
				}
				catch
				{
					// just suppress any error logging exceptions
				}

			}

		}
	}
}
