using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FridayClean.Client.Helpers
{
	partial class Utils
	{
		public static string PhoneTrimer(string phone)
		{
			return Regex.Replace(phone, "[^0-9]", "");
		}
	}
}
