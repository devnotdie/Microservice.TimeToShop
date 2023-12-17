using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using Identity.API.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Identity.API.Pages.Logout
{
	[SecurityHeaders]
	[AllowAnonymous]
	public class Index : PageModel
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IIdentityServerInteractionService _interactionService;
		private readonly IEventService _events;

		[BindProperty]
		public string LogoutId { get; set; }

		public Index(SignInManager<ApplicationUser> signInManager, IIdentityServerInteractionService interactionService, IEventService events)
		{
			_signInManager = signInManager;
			_interactionService = interactionService;
			_events = events;
		}

		public async Task<IActionResult> OnGet(string logoutId)
		{
			LogoutId = logoutId;

			if (User.Identity.IsAuthenticated)
			{
				return await OnPost();
			}

			return Redirect("~/");
		}

		public async Task<IActionResult> OnPost()
		{
			if (User.Identity.IsAuthenticated)
			{
				LogoutId ??= await _interactionService.CreateLogoutContextAsync();
				var logout = await _interactionService.GetLogoutContextAsync(LogoutId);

				await _signInManager.SignOutAsync();
				await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));

				var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
				if (idp != null && idp != Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider)
				{
					if (await HttpContext.GetSchemeSupportsSignOutAsync(idp))
					{
						return SignOut(new AuthenticationProperties { RedirectUri = logout.PostLogoutRedirectUri }, idp);
					}
				}

				if (!string.IsNullOrWhiteSpace(logout?.PostLogoutRedirectUri))
				{
					return Redirect(logout.PostLogoutRedirectUri);
				}
			}

			return Redirect("~/");
		}
	}
}