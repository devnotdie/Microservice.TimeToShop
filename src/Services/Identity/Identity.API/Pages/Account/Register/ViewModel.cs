using Identity.API.Pages.Account.Shared.Models;

namespace Identity.API.Pages.Account.Register
{
	public class ViewModel
	{
		public IEnumerable<ExternalProviderViewModel> ExternalProviders { get; set; } = [];
	}
}
