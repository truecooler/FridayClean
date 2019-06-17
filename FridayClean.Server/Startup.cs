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
			var logger =
				serviceProvider.GetService(typeof(ILogger<FridayCleanServiceSettings>)) 
					as ILogger<FridayCleanServiceSettings>;
			var settings = FridayCleanServiceSettings.LoadOrCreateDefault(logger);

			services.AddSingleton<FridayCleanServiceSettings>(settings);


			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseNpgsql(settings.PostgresqlConnectionString);
			});

			services.AddScoped<IRepository<User>, BaseRepository<User, ApplicationDbContext>>();
			services.AddScoped<IRepository<SentSmsCode>, BaseRepository<SentSmsCode, ApplicationDbContext>>();
			services.AddScoped<IRepository<AuthenticatedSession>, BaseRepository<AuthenticatedSession, ApplicationDbContext>>();
			services.AddScoped<IRepository<DataBaseModels.CleaningService>, BaseRepository<DataBaseModels.CleaningService, ApplicationDbContext>>();
			services.AddScoped<IRepository<DataBaseModels.OrderedCleaning>, BaseRepository<DataBaseModels.OrderedCleaning, ApplicationDbContext>>();
			serviceProvider = services.BuildServiceProvider();

			var dbcontext = serviceProvider.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
			dbcontext.Database.EnsureCreated();
			

			var authenticatedSessionsRepository = (IRepository<AuthenticatedSession>)serviceProvider.GetService(typeof(IRepository<AuthenticatedSession>));

			Action<ServerCallContext> callback = (context) =>
			{
				if (context.Method.StartsWith("/FridayClean.FridayCleanCommunication/Auth"))
				{
					return;
				}

				var accessToken = context.RequestHeaders.SingleOrDefault(x => x.Key == Constants.AuthHeaderName)?.Value;

				if (string.IsNullOrEmpty(accessToken))
				{
					throw new RpcException(new Status(StatusCode.Unauthenticated,"This rpc method requires access token."));
				}

				if (!authenticatedSessionsRepository.IsExist(x => x.AccessToken == accessToken))
				{
					throw new RpcException(new Status(StatusCode.Unauthenticated, "Provided access token not exists or expired."));
				}

				var phone = authenticatedSessionsRepository.Get(x => x.AccessToken == accessToken)?.Phone;

				if (string.IsNullOrEmpty(phone))
				{
					throw new ApplicationException("Fatal: user auth session exists, but phone doesn't.");
				}

				context.RequestHeaders.Add(Constants.UserPhoneHeaderName,phone);

			};

			services.AddSingleton<AuthInterceptor>(new AuthInterceptor(callback));

			

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
