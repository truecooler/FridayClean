using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FridayClean.Server.DataBaseModels
{
	public class AuthenticatedSession
	{
		//[Key]
		public string Phone { get; set; }
		public string AccessToken { get; set; }
		public User User { get; set; }
	}
}
