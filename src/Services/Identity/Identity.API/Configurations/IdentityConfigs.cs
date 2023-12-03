using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace Identity.API.Configurations
{
	public static class IdentityConfigs
	{
		public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
		{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile()
		};

		public static IEnumerable<ApiResource> GetApiResources => new ApiResource[]
		{
			new ("user", "User Service")
			{
				Scopes = { "web"}
			},
			new ("order", "Order Service")
			{
				Scopes = { "web"}
			},
			new ("catalog", "Catalog Service")
			{
				Scopes = { "web"}
			},
			new ("basket", "Basket Service")
			{
				Scopes = { "web"}
			}
		};

		public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
		{
			new ("web")
		};

		public static IEnumerable<Client> Clients => new Client[]
		{
			new() {
				ClientId = "time-to-shop-webapp",
				//ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },
				RequireClientSecret = false,
				AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
				RedirectUris = { "https://localhost:44300/signin-oidc" },
				FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
				PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },
				AllowOfflineAccess = true,
				AllowedScopes = {"web"}
			},
		};
	}
}
