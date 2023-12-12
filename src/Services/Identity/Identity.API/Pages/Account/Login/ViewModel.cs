// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.
using Identity.API.Pages.Account.Shared.Models;

namespace Identity.API.Pages.Login
{
	public class ViewModel
	{
		public IEnumerable<ExternalProviderViewModel> ExternalProviders { get; set; } = [];
	}
}