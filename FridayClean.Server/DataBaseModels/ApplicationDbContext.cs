using FridayClean.Server.DataBaseModels.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FridayClean.Common;

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
			modelBuilder.ApplyConfiguration(new CleaningServiceConfiguration());
			modelBuilder.ApplyConfiguration(new OrderedCleaningConfiguration());
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
				builder.Property(e => e.Role)
					.HasConversion(
						v => v.ToString(),
						v => (UserRole) Enum.Parse(typeof(UserRole), v));
				//.HasForeignKey<SentSmsCode>(z => z.Phone);

				builder.HasMany(x => x.OrderedCleaningsByCustomer)
					.WithOne(x => x.Customer)
					.HasForeignKey(x => x.CustomerPhone)
					.OnDelete(DeleteBehavior.SetNull);

				builder.HasMany(x => x.ObservedCleaningsByCleaner)
					.WithOne(x => x.Cleaner)
					.HasForeignKey(x => x.CleanerPhone)
					.OnDelete(DeleteBehavior.SetNull);

				builder.HasData(new User[]
				{
					new User()
					{
						Name = "Тамара",
						Phone = "79855439087",
						AvatarLink = "https://pp.userapi.com/c824701/v824701171/1557a9/gPMy38gq8Mo.jpg",
						Money = 0,
						Role = UserRole.Cleaner
					},
					new User()
					{
						Name = "Егор",
						Phone = "79458439027",
						AvatarLink = "https://pp.userapi.com/c856124/v856124851/6a87c/5YtDkKBthNo.jpg",
						Money = 0,
						Role = UserRole.Cleaner
					},
					new User()
					{
						Name = "Елена",
						Phone = "79154339282",
						AvatarLink = "https://pp.userapi.com/c845418/v845418531/15ea60/LHdACnxrGys.jpg",
						Money = 0,
						Role = UserRole.Cleaner
					},
					new User()
					{
						Name = "Сергей",
						Phone = "79755639081",
						AvatarLink = "https://pp.userapi.com/c850132/v850132416/2ccbe/bktlyZOIF3w.jpg",
						Money = 0,
						Role = UserRole.Cleaner
					},
					new User()
					{
						Name = "Алексей",
						Phone = "78751339281",
						AvatarLink = "https://img.tsargrad.tv/cache/a/a/20190203_gaf_ru04_055.jpg/w1056h594fill.jpg",
						Money = 0,
						Role = UserRole.Cleaner
					}

				});

				builder.HasKey(o => o.Phone);
				//builder.Property(x => x.Address).IsRequired();
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
				builder.HasKey(table => new {table.Phone, table.AccessToken});
			}
		}


		public class CleaningServiceConfiguration : IEntityTypeConfiguration<CleaningService>
		{
			public void Configure(EntityTypeBuilder<CleaningService> builder)
			{
				builder.ToTable("CleaningServices");
				//.HasOne(x => x.User)
				//.WithOne(y => y.SentSmsCode)
				//.HasForeignKey<SentSmsCode>(z => z.Phone);
				builder.HasKey(x => x.CleaningType);
				builder.HasMany(x => x.OrderedCleanings)
					.WithOne(x => x.CleaningService)
					.HasForeignKey(x => x.CleaningType);
					//.OnDelete(DeleteBehavior.SetNull);

				builder.Property(e => e.CleaningType)
					.HasConversion(
						v => v.ToString(),
						v => (CleaningType) Enum.Parse(typeof(CleaningType), v));

				List<CleaningService> data = new List<CleaningService>()
				{
					new CleaningService()
					{
						CleaningType = CleaningType.MaintenanceCleaning,
						Name = "Поддерживающая уборка",
						ApartmentAreaMin = 40,
						ApartmentAreaMax = 80,
						StartingPrice = 2000,
						Description = Constants.Descriptions.MaintenanceCleaning

					},
					new CleaningService()
					{
						CleaningType = CleaningType.ComplexCleaning,
						Name = "Комплексная уборка",
						ApartmentAreaMin = 40,
						ApartmentAreaMax = 80,
						StartingPrice = 3200,
						Description = Constants.Descriptions.ComplexCleaning
					},
					new CleaningService()
					{
						CleaningType = CleaningType.GeneralCleaning,
						Name = "Генеральная уборка",
						ApartmentAreaMin = 40,
						ApartmentAreaMax = 80,
						StartingPrice = 6000,
						Description = Constants.Descriptions.GeneralCleaning
					}

				};


				builder.HasData(data);
			}
		}

		public class OrderedCleaningConfiguration : IEntityTypeConfiguration<OrderedCleaning>
		{
			public void Configure(EntityTypeBuilder<OrderedCleaning> builder)
			{
				builder.ToTable("OrderedCleanings");
				//.HasOne(x => x.)
				//.WithOne(y => y.SentSmsCode)
				//.HasForeignKey<SentSmsCode>(z => z.Phone);
				builder.HasKey(x => x.Id);

				builder.Property(x => x.CleaningType);
					//.IsRequired(false);

				builder.Property(e => e.CleaningType)
					.HasConversion(
						v => v.ToString(),
						v => (CleaningType)Enum.Parse(typeof(CleaningType), v));

				builder.Property(e => e.State)
					.HasConversion(
						v => v.ToString(),
						v => (OrderedCleaningState)Enum.Parse(typeof(OrderedCleaningState), v));

			}
		}
	}

}
