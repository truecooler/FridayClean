using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridayClean.Server.DataBaseModels
{



	public class OrderedCleaning
	{
		public int Id { get; set; }

		public string CustomerPhone { get; set; }

		public string CleanerPhone { get; set; }

		public CleaningType CleaningType { get; set; }
		public int ApartmentArea { get; set; }
		public int Price { get; set; }
		public OrderedCleaningState State { get; set; }

		public User Customer { get; set; }
		public User Cleaner { get; set; }
	}
}
