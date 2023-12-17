using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace Identity.API.Pages.ExternalLogin
{
	[AllowAnonymous]
	[SecurityHeaders]
	public class Challenge : PageModel
	{
		private readonly IIdentityServerInteractionService _interactionService;

		public Challenge(IIdentityServerInteractionService interactionService)
		{
			_interactionService = interactionService;
		}

		public IActionResult OnGet(string scheme, string returnUrl)
		{
			if (string.IsNullOrWhiteSpace(returnUrl))
			{
				returnUrl = "~/";
			}

			if (!Url.IsLocalUrl(returnUrl) && !_interactionService.IsValidReturnUrl(returnUrl))
			{
				throw new Exception("invalid return URL");
			}

			var properties = new AuthenticationProperties
			{
				AllowRefresh = true,
				RedirectUri = Url.Page("/externallogin/callback"),
				Items =
				{
					{ "returnUrl", returnUrl },
					{ "scheme", scheme },
				}
			};

			return Challenge(properties, scheme);
		}
	}
}