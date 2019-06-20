using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FridayClean.Server.DataBaseModels.Enums;

namespace FridayClean.Server.DataBaseModels
{
	public class CleaningService
	{
		public CleaningType CleaningType { get; set; }

		public int ApartmentAreaMin { get; set; }
		public int ApartmentAreaMax { get; set; }

		public int StartingPrice { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public ICollection<OrderedCleaning> OrderedCleanings { get; set; }
	}
}
