using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FridayClean.Server.SmsService;
using Grpc.AspNetCore.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace FridayClean.Server
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddGrpc();
			services.AddSingleton<ISmsService, SmscSmsService>();
			services.AddSingleton<IRestClient, RestClient>();

			/* very ugly way, don't use in production */
			var serviceProvider = services.BuildServiceProvider();
			var logger = serviceProvider.GetService(typeof(ILogger<FridayCleanServiceSettings>));
			var settings = FridayCleanServiceSettings.LoadOrCreateDefault((ILogger<FridayCleanServiceSettings>)logger);

			services.AddSingleton<FridayCleanServiceSettings>(settings);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

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
