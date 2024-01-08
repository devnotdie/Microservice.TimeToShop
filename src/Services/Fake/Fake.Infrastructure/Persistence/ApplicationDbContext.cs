using Fake.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Fake.Infrastructure.Persistence
{
	internal class ApplicationDbContext : DbContext, IApplicationDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
			//Database.EnsureCreated();
		}

		public Task SaveChangesAsync()
		{
			return base.SaveChangesAsync();
		}
	}
}
