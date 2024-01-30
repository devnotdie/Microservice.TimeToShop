using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading.Tasks;

namespace BuildingBlocks.Common.Extensions
{
	public static class CommonExtensions
	{
		public static IHostBuilder UseLogging(this IHostBuilder hostBuilder, IWebHostEnvironment env)
		{
			hostBuilder.UseSerilog((HostBuilderContext context, LoggerConfiguration configuration) =>
			{
				configuration
					.WriteTo.Console(outputTemplate: "[{Elapsed} F{TEST}F {Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
					.WriteTo.Seq(context.Configuration["SeqUrl"])
					.Enrich.WithProperty("ApplicationName", env.ApplicationName)
					.Enrich.WithProperty("MachineName", Environment.MachineName)
					.Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
					.Enrich.FromLogContext()
					.ReadFrom.Configuration(context.Configuration);
			});

			return hostBuilder;
		}

		public static IServiceCollection AddDefaultAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
			// {
			//   "Identity": {
			//     "Url": "http://identity",
			//     "Audience": "basket"
			//    }
			// }

			var identitySection = configuration.GetSection("Identity");

			if (!identitySection.Exists())
			{
				// No identity section, so no authentication
				return services;
			}

			// prevent from mapping "sub" claim to nameidentifier.
			//JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

			services.AddAuthentication().AddJwtBearer(options =>
			{
				options.Authority = identitySection.GetValue<string>("Url");
				options.RequireHttpsMetadata = false;
				options.Audience = identitySection.GetValue<string>("Audience");
				options.TokenValidationParameters.ValidateAudience = false;
			});

			return services;
		}

		public static async Task<bool> CheckHealthAsync(this WebApplication app)
		{
			app.Logger.LogInformation("Running health checks...");

			// Do a health check on startup, this will throw an exception if any of the checks fail
			var report = await app.Services.GetRequiredService<HealthCheckService>().CheckHealthAsync();

			if (report.Status == HealthStatus.Unhealthy)
			{
				app.Logger.LogCritical("Health checks failed!");
				foreach (var entry in report.Entries)
				{
					if (entry.Value.Status == HealthStatus.Unhealthy)
					{
						app.Logger.LogCritical("{Check}: {Status}", entry.Key, entry.Value.Status);
					}
				}

				return false;
			}

			return true;
		}
	}
}
