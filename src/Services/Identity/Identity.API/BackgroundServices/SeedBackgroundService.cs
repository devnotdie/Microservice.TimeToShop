using IdentityModel;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Identity.API.BackgroundServices
{
	public class SeedBackgroundService : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;

		public SeedBackgroundService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await using var scope = _serviceProvider.CreateAsyncScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

			var email = "oleh@gmail.com";
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
				var result = await userManager.CreateAsync(user, "Pass123$");
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
