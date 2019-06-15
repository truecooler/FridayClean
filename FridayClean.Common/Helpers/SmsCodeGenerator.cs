using System;
using System.Collections.Generic;
using System.Text;

namespace FridayClean.Common.Helpers
{
	public partial class Utils
	{
		public class SmsCodeGenerator
		{
			public static int Generate()
			{
				return new Random().Next(10000,99999);
			}
		}
	}
}
