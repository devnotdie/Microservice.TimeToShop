using Identity.API.Pages.Account.Shared.Models;
using System.Collections.Generic;

namespace Identity.API.Pages.Login
{
	public class ViewModel
	{
		public IEnumerable<ExternalProviderViewModel> ExternalProviders { get; set; } = [];
	}
}