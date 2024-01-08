using Fake.Application.Common.Abstractions;
using Fake.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fake.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseNpgsql("", builder =>
				{
					builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
				});
			});

			services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

			return services;
		}
	}
}
