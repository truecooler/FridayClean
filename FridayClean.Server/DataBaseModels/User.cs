using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using FridayClean.Server.DataBaseModels.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FridayClean.Server.DataBaseModels
{
	public class User
	{
		public string Phone { get; set; }

		public string Name { get; set; }

		public string Address { get; set; }

		public int Money { get; set; }

		public UserRole Role{get ; set; }

		public ICollection<AuthenticatedSession> AuthenticatedSessions { get; set; }

		public ICollection<OrderedCleaning> OrderedCleaningsByCustomer { get; set; }
		public ICollection<OrderedCleaning> ObservedCleaningsByCleaner { get; set; }

	}
}
