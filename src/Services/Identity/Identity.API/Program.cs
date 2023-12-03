using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.Services;
using Identity.API.Configurations;
using Identity.API.Data;
using Identity.API.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Linq;
using Duende.IdentityServer.EntityFramework.Mappers;

namespace Identity.API
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
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
				.AddConfigurationStore<ApplicationDbContext>()
				.AddOperationalStore<ApplicationDbContext>()
				.AddAspNetIdentity<ApplicationUser>();
			//.AddProfileService<DefaultProfileService>();

			//.AddInMemoryIdentityResources(IdentityConfigs.IdentityResources)
			//.AddInMemoryApiScopes(IdentityConfigs.ApiScopes)
			//.AddInMemoryApiResources(IdentityConfigs.GetApiResources)
			//.AddInMemoryClients(IdentityConfigs.Clients)
			//.AddAspNetIdentity<ApplicationUser>()
			//.AddProfileService<DefaultProfileService>();		

			//.AddConfigurationStore(options =>
			//{
			//	options.ConfigureDbContext = optionsBuilder =>
			//	{
			//		optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("ConfigurationConnection"));
			//	};
			//})
			//	.AddOperationalStore(options =>
			//	 {
			//		 options.ConfigureDbContext = optionsBuilder =>
			//		 {
			//			 optionsBuilder.UseSqlServer("PersistedGrantConnection");
			//		 };
			//	 })

			var app = builder.Build();

			app.MapGet("/", () => "Hello World!");

			app.UseIdentityServer();
			app.UseAuthorization();

			await app.RunAsync();
		}
	}
}
