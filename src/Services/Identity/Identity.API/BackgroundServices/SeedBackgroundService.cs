using Identity.API.Data;
using Identity.API.Services.User;
using Identity.API.Services.User.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
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
			var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

			await context.Database.MigrateAsync(stoppingToken);

			var email = _configuration["Seed:AdminEmail"];
			var password = _configuration["ADMIN_PASSWORD"];
			var roleName = "Admin";

			var result = await userService.AddUserAsync(new CreateUserModel
			{
				Email = email,
				Password = password,
				FirstName = "Admin",
				LastName = "Admin"
			}, [roleName]);
		}
	}
}
