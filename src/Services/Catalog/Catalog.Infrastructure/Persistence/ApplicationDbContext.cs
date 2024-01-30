using Catalog.Application.Abstractions;
using Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Persistence
{
	internal class ApplicationDbContext : DbContext, IApplicationDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<CatalogType> CatalogTypes => Set<CatalogType>();

		public DbSet<CatalogBrand> CatalogBrands => Set<CatalogBrand>();

		public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();

		public Task SaveChangesAsync()
		{
			return base.SaveChangesAsync(CancellationToken.None);
		}
	}
}
