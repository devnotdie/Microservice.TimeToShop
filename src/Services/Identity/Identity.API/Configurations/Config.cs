using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Identity.API.Configurations
{
	public static class Config
	{
		public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
		{
			new IdentityResources.OpenId(),
			new IdentityResource("roles", new[] { "role" })

		};

		public static IEnumerable<ApiResource> GetApiResources => new ApiResource[]
		{
		};

		public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
		{
			new ("web.full", "Full web access to services"),
		};

		public static IEnumerable<Client> Clients(IConfiguration configuration) => new Client[]
		{
			new()
			{
				AccessTokenLifetime = 60,
				ClientId = "webapp",
				RequireClientSecret = false,
				AllowedGrantTypes = GrantTypes.Code,
				RedirectUris = { $"{configuration["WebClient"]}/signin-oidc" },
				FrontChannelLogoutUri = $"{configuration["WebClient"]}/signout-oidc",
				PostLogoutRedirectUris = { $"{configuration["WebClient"]}/home" },
				AllowOfflineAccess = true,
				AllowedCorsOrigins = { $"{configuration["WebClient"]}" },
				AllowedScopes =
				{
					IdentityServerConstants.StandardScopes.OpenId,
					"roles",
					"web.full"
				}
			},
		};
	}
}
