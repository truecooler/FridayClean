﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FridayClean.Server.DataBaseModels
{
	[Table(name:"Users")]
	public class User
	{
		[Key]
		public int Id;

		public string Phone;

		public string Name;
	}
}
