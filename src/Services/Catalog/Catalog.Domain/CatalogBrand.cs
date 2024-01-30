using System.ComponentModel.DataAnnotations;

namespace Catalog.Domain
{
	public class CatalogBrand
	{
		public int Id { get; set; }

		[MaxLength(100)]
		public string Title { get; set; }
	}
}
