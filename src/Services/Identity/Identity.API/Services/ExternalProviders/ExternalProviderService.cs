using Duende.IdentityServer.Stores;
using Identity.API.Services.ExternalProviders.Models;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Services.ExternalProviders
{
	public interface IExternalProviderService
	{
		Task<ICollection<ExternalProviderModel>> GetAllAsync();
	}

	public class ExternalProviderService : IExternalProviderService
	{
		private readonly IAuthenticationSchemeProvider _schemeProvider;
		private readonly IIdentityProviderStore _identityProviderStore;

		public ExternalProviderService(
			IAuthenticationSchemeProvider schemeProvider,
			IIdentityProviderStore identityProviderStore)
		{
			_schemeProvider = schemeProvider;
			_identityProviderStore = identityProviderStore;
		}

		public async Task<ICollection<ExternalProviderModel>> GetAllAsync()
		{
			var providers = (await _schemeProvider.GetAllSchemesAsync())
				.Where(x => x.DisplayName != null)
				.Select(x => new ExternalProviderModel
				{
					DisplayName = x.DisplayName ?? x.Name,
					AuthenticationScheme = x.Name
				});

			var dynamicSchemes = (await _identityProviderStore.GetAllSchemeNamesAsync())
				.Where(x => x.Enabled)
				.Select(x => new ExternalProviderModel
				{
					AuthenticationScheme = x.Scheme,
					DisplayName = x.DisplayName
				});

			return providers
				.Union(dynamicSchemes)
				.ToArray();
		}
	}
}
