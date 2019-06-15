using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FridayClean.Common.Helpers
{
	public partial class Utils
	{
		public static class AccessTokenGenerator
		{
			private static string ComputeSha256Hash(string rawData)
			{
				// Create a SHA256   
				using (SHA256 sha256Hash = SHA256.Create())
				{
					// ComputeHash - returns byte array  
					byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

					// Convert byte array to a string   
					StringBuilder builder = new StringBuilder();
					for (int i = 0; i < bytes.Length; i++)
					{
						builder.Append(bytes[i].ToString("x2"));
					}
					return builder.ToString();
				}
			}

			public static string Generate(string phone,int code)
			{
				var chunk1 = ComputeSha256Hash(phone);
				var chunk2 = ComputeSha256Hash(code.ToString());
				var chunk3 = ComputeSha256Hash(Guid.NewGuid().ToString());

				return ComputeSha256Hash($"{chunk1}{chunk2}{chunk3}");
			}
		}
	}
}
