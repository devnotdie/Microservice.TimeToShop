using Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Catalog.Application.Abstractions
{
	public interface IApplicationDbContext
	{
		public DbSet<CatalogType> CatalogTypes { get; }

		public DbSet<CatalogBrand> CatalogBrands { get; }

		public DbSet<CatalogItem> CatalogItems { get; }

		Task SaveChangesAsync();
	}
}
