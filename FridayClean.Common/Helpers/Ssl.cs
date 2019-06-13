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

			public static string ClientCertAuthorityPath
			{
				get { return GetPath("Certificates/ca.pem"); }
			}

			public static string ServerCertChainPath
			{
				get { return GetPath("Certificates/server1.pem"); }
			}

			public static string ServerPrivateKeyPath
			{
				get { return GetPath("Certificates/server1.key"); }
			}

			public static SslCredentials CreateSslClientCredentials()
			{
				return new SslCredentials(File.ReadAllText(ClientCertAuthorityPath));
			}

			public static SslServerCredentials CreateSslServerCredentials()
			{
				var keyCertPair = new KeyCertificatePair(
					File.ReadAllText(ServerCertChainPath),
					File.ReadAllText(ServerPrivateKeyPath));
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
