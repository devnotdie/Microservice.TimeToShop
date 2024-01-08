using Identity.API.Data;
using Identity.API.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.API.BackgroundServices
{
	public class SeedBackgroundService : BackgroundService
	{
		private readonly IConfiguration _configuration;
		private readonly IServiceProvider _serviceProvider;

		public SeedBackgroundService(
			IConfiguration configuration,
			IServiceProvider serviceProvider)
		{
			_configuration = configuration;
			_serviceProvider = serviceProvider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var enable = _configuration.GetValue<bool>("Seed:Enable");
			if (!enable)
			{
				return;
			}

			await using var scope = _serviceProvider.CreateAsyncScope();
			var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

			await context.Database.MigrateAsync();

			var email = _configuration["Seed:AdminEmail"];
			var password = _configuration["ADMIN_PASSWORD"];
			var roleName = "Admin";

			var role = await roleManager.FindByNameAsync(roleName);
			if (role == null)
			{
				role = new ApplicationRole(roleName);
				var result = await roleManager.CreateAsync(role);
				if (!result.Succeeded)
				{
					throw new Exception(result.Errors.First().Description);
				}
			}

			var user = await userManager.FindByNameAsync(email);
			if (user == null)
			{
				user = new ApplicationUser(email)
				{
					EmailConfirmed = true,
				};
				var result = await userManager.CreateAsync(user, password);
				if (!result.Succeeded)
				{
					throw new Exception(result.Errors.First().Description);
				}

				result = await userManager.AddToRoleAsync(user, roleName);
				if (!result.Succeeded)
				{
					throw new Exception(result.Errors.First().Description);
				}

				result = await userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Role, roleName));
				if (!result.Succeeded)
				{
					throw new Exception(result.Errors.First().Description);
				}
			}
		}
	}
}
