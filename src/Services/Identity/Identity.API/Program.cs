using Duende.IdentityServer;
using Identity.API.BackgroundServices;
using Identity.API.Configurations;
using Identity.API.Data;
using Identity.API.Models;
using Identity.API.Services.ExternalProviders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Identity.API
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

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

			var app = builder.Build();

			app.UseStaticFiles();
			app.UseRouting();
			app.UseIdentityServer();
			app.UseAuthorization();

			app.MapRazorPages();

			await app.RunAsync();
		}
	}
}
