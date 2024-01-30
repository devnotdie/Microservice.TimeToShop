using System.ComponentModel.DataAnnotations;

namespace Catalog.Domain
{
	public class CatalogType
	{
		public int Id { get; set; }

		[MaxLength(100)]
		public string Title { get; set; }
	}
}
