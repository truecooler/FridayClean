using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridayClean.Server.DataBaseModels
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext()
		{
			Database.EnsureCreated();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>().ToTable("Users");
		}

	}
}
