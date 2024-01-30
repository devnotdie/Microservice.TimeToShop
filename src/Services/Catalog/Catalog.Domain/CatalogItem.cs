using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.Domain
{
	public class CatalogItem
	{
		public int Id { get; set; }

		public int BrandId { get; set; }

		[ForeignKey(nameof(BrandId))]
		public CatalogBrand Brand { get; set; }

		public int TypeId { get; set; }

		[ForeignKey(nameof(TypeId))]
		public CatalogType Type { get; set; }

		[MaxLength(100)]
		public string Title { get; set; }

		[MaxLength(300)]
		public string Description { get; set; }

		public decimal Price { get; set; }

		[MaxLength(100)]
		public string ImageName { get; set; }

		[Length(0, 100)]
		public int AvailableStock { get; set; }
	}
}
