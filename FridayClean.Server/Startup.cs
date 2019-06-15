using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FridayClean.Common;
using FridayClean.Common.Interceptors;
using FridayClean.Server.Repositories;
using FridayClean.Server.SmsService;
using Grpc.AspNetCore.Server;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSharp;
using FridayClean.Server.DataBaseModels;

namespace FridayClean.Server
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddGrpc().AddServiceOptions<FridayCleanService>(options => options.Interceptors.Add<AuthInterceptor>());
			
			services.AddSingleton<ISmsService, SmscSmsService>();
			services.AddSingleton<IRestClient, RestClient>();

			/* very ugly way, don't use in production */
			var serviceProvider = services.BuildServiceProvider();
			ILogger<FridayCleanServiceSettings> logger =
				(ILogger<FridayCleanServiceSettings>)serviceProvider.GetService(typeof(ILogger<FridayCleanServiceSettings>));
			var settings = FridayCleanServiceSettings.LoadOrCreateDefault(logger);

			services.AddSingleton<FridayCleanServiceSettings>(settings);

			Action<ServerCallContext> callback = (context) =>
			{
				
			};

			services.AddSingleton<AuthInterceptor>(new AuthInterceptor(callback));

			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseNpgsql(settings.PostgresqlConnectionString);
			});

			services.AddScoped<IRepository<User>, BaseRepository<User, ApplicationDbContext>>();
			services.AddScoped<IRepository<SentSmsCode>, BaseRepository<SentSmsCode, ApplicationDbContext>>();
			services.AddScoped<IRepository<AuthenticatedSession>, BaseRepository<AuthenticatedSession, ApplicationDbContext>>();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				// Communication with gRPC endpoints must be made through a gRPC client.
				// To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909
				endpoints.MapGrpcService<FridayCleanService>();
			});
		}
	}
}
