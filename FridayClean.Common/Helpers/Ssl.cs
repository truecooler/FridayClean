using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace FridayClean.Common.Helpers
{
	public partial class Utils
	{
		public class Ssl
		{
			public const string DefaultHostOverride = "foo.test.google.fr";

			private static Assembly GetCurrentAssembly()
			{
				return typeof(Ssl).Assembly;
			}

			private static string ReadCertDataFromResource(string resourceName)
			{
				return new StreamReader(GetCurrentAssembly().GetManifestResourceStream(resourceName)).ReadToEnd();
			}
			public static string GetClientCertAuthorityData()
			{
				return ReadCertDataFromResource("FridayClean.Common.Certificates.ca.pem"); //("Certificates/ca.pem"); 
			}

			public static string GetServerCertChainData()
			{
				return ReadCertDataFromResource("FridayClean.Common.Certificates.server1.pem"); //("Certificates/server1.pem"); }
			}

			public static string GetServerPrivateKeyData()
			{
				return ReadCertDataFromResource("FridayClean.Common.Certificates.server1.key");
				//return GetPath("Certificates/server1.key");
			}

			public static SslCredentials CreateSslClientCredentials()
			{
				return new SslCredentials(GetClientCertAuthorityData());
			}

			public static SslServerCredentials CreateSslServerCredentials()
			{
				var keyCertPair = new KeyCertificatePair(
					GetServerCertChainData(),
					GetServerPrivateKeyData());
				return new SslServerCredentials(new[] {keyCertPair});
			}

			private static string GetPath(string relativePath)
			{
				var assemblyDir = Path.GetDirectoryName(typeof(SslCredentials).GetTypeInfo().Assembly.Location);
				return Path.Combine(assemblyDir, relativePath);
			}
		}
	}
}
