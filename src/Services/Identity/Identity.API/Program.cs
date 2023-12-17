using Duende.IdentityServer;
using Identity.API.BackgroundServices;
using Identity.API.Configurations;
using Identity.API.Data;
using Identity.API.Models;
using Identity.API.Services.ExternalProviders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Identity.API
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddRazorPages();
			//builder.Services.AddAntiforgery(options => { options.SuppressXFrameOptionsHeader = true; });

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
					//options.IssuerUri = "null";
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

			builder.Services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>();

			var app = builder.Build();
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}

			app.UseStaticFiles();
			app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });	
			app.UseRouting();
			app.UseIdentityServer();
			app.UseAuthorization();

			app.MapRazorPages();
			app.MapHealthChecks("/health");
			app.MapGet("/test", (HttpContext context) =>
			{
				//var c = context.RequestServices.GetRequiredService<IConfiguration>();
				//var connectionString = c.GetConnectionString("DefaultConnection");
				//var ss = string.Format(connectionString, builder.Configuration["DB_PASSWORD"]);	
				//return $"G - {ss}";

				var c = context.RequestServices.GetRequiredService<ApplicationDbContext>();
				var r = c.Database.CanConnect();
				return $"DBCONNECT - {r}";
			});

			await app.RunAsync();
		}
	}
}
