using Identity.API.Pages.Account.Shared.Models;
using System.Collections.Generic;

namespace Identity.API.Pages.Account.Register
{
	public class ViewModel
	{
		public IEnumerable<ExternalProviderViewModel> ExternalProviders { get; set; } = [];
	}
}
