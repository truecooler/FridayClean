using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FridayClean.Server.DataBaseModels
{
	//[Table(name: "SentSmsCodes")]
	public class SentSmsCode
	{
		//[Key]
		//[ForeignKey("User")]
		public string Phone { get; set; }
		public int Code { get; set; }
		//public User User { get; set; }
	}
}
