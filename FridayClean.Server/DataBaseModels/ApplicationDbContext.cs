using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridayClean.Server.DataBaseModels
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
			Database.EnsureCreated();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new UserConfiguration());
			modelBuilder.ApplyConfiguration(new SentSmsCodeConfiguration());
			modelBuilder.ApplyConfiguration(new AuthenticatedSessionConfiguration());
		}

		public class UserConfiguration : IEntityTypeConfiguration<User>
		{
			public void Configure(EntityTypeBuilder<User> builder)
			{
				builder.ToTable("Users")
					.HasMany(x => x.AuthenticatedSessions)
					.WithOne(x => x.User)
					.HasForeignKey(x => x.Phone)
					.OnDelete(DeleteBehavior.Cascade);
				//.HasForeignKey<SentSmsCode>(z => z.Phone);
				builder.HasKey(o => o.Phone);
			}
		}

		public class SentSmsCodeConfiguration : IEntityTypeConfiguration<SentSmsCode>
		{
			public void Configure(EntityTypeBuilder<SentSmsCode> builder)
			{
				builder.ToTable("SentSmsCodes");
					//.HasOne(x => x.User)
					//.WithOne(y => y.SentSmsCode)
					//.HasForeignKey<SentSmsCode>(z => z.Phone);
				builder.HasKey(o => o.Phone);
			}
		}

		public class AuthenticatedSessionConfiguration : IEntityTypeConfiguration<AuthenticatedSession>
		{
			public void Configure(EntityTypeBuilder<AuthenticatedSession> builder)
			{
				builder.ToTable("AuthenticatedSessions");
				//.HasOne(x => x.User)
				//.WithOne(y => y.SentSmsCode)
				//.HasForeignKey<SentSmsCode>(z => z.Phone);
				builder.HasKey(table => new {table.Phone,table.AccessToken});
			}
		}

	}


}
