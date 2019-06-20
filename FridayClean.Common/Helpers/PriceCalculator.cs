using System;
using System.Collections.Generic;
using System.Text;

using FridayClean;

namespace FridayClean.Common.Helpers
{
	public partial class Utils
	{
		public static class PriceCalculator
		{
			public static int Calculate(int apartamentAreaMin, int apartamentArea, int startingPrice)
			{
				return Convert.ToInt32((Convert.ToSingle(apartamentArea- (apartamentArea % Constants.PriceStep)) / Convert.ToSingle(apartamentAreaMin)) *
				                       Convert.ToSingle(startingPrice));
			}
		}
	}
}
