using Duende.IdentityServer;
using Identity.API.BackgroundServices;
using Identity.API.Configurations;
using Identity.API.Data;
using Identity.API.Grpc;
using Identity.API.Models;
using Identity.API.Services.ExternalProviders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Identity.API
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Logging.AddFilter("Grpc", LogLevel.Debug);

			builder.Services.AddGrpc();
			builder.Services.AddGrpcReflection();
			builder.Services.AddRazorPages();
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
			{
				var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
				options.UseSqlServer(string.Format(connectionString, builder.Configuration["DB_PASSWORD"]));
			});

			builder.Services
				.AddIdentity<ApplicationUser, ApplicationRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			builder.Services
				.AddIdentityServer(options =>
				{
					options.Events.RaiseErrorEvents = true;
					options.Events.RaiseInformationEvents = true;
					options.Events.RaiseFailureEvents = true;
					options.Events.RaiseSuccessEvents = true;
				})
				.AddInMemoryIdentityResources(Config.IdentityResources)
				.AddInMemoryApiScopes(Config.ApiScopes)
				.AddInMemoryApiResources(Config.GetApiResources)
				.AddInMemoryClients(Config.Clients(builder.Configuration))
				.AddAspNetIdentity<ApplicationUser>()
				.AddDeveloperSigningCredential();

			builder.Services
				.AddAuthentication()
				.AddGoogle(options =>
				{
					// register your IdentityServer with Google at https://console.developers.google.com
					// enable the Google+ API
					// set the redirect URI to https://localhost:5001/signin-google

					options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
					options.ClientId = builder.Configuration["ExternalProviders:GoogleClientId"];
					options.ClientSecret = builder.Configuration["GOOGLE_SECRET"];
				});

			builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
			builder.Services.AddHostedService<SeedBackgroundService>();
			builder.Services.AddScoped<IExternalProviderService, ExternalProviderService>();

			builder.Services
				.AddHealthChecks()
				.AddDbContextCheck<ApplicationDbContext>();

			var app = builder.Build();
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
				app.MapGrpcReflectionService();
			}

			app.UseStaticFiles();
			app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
			app.UseRouting();
			app.UseIdentityServer();
			app.UseAuthorization();

			app.MapGrpcService<UserGrpcService>();
			app.MapRazorPages();
			app.MapHealthChecks("/health");
			await app.RunAsync();
		}
	}
}
