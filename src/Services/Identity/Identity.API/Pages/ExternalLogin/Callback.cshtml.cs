using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Identity.API.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.API.Pages.ExternalLogin
{
	[AllowAnonymous]
	[SecurityHeaders]
	public class Callback : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IIdentityServerInteractionService _interaction;
		private readonly ILogger<Callback> _logger;
		private readonly IEventService _events;

		public Callback(
			IIdentityServerInteractionService interaction,
			IEventService events,
			ILogger<Callback> logger,
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_interaction = interaction;
			_logger = logger;
			_events = events;
		}

		public async Task<IActionResult> OnGet()
		{
			var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
			if (result?.Succeeded != true)
			{
				throw new Exception("External authentication error");
			}

			var externalUser = result.Principal;

			if (_logger.IsEnabled(LogLevel.Debug))
			{
				var externalClaims = externalUser.Claims.Select(c => $"{c.Type}: {c.Value}");
				_logger.LogDebug("External claims: {@claims}", externalClaims);
			}

			var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject)
				?? externalUser.FindFirst(ClaimTypes.NameIdentifier)
				?? throw new Exception("Unknown userid");

			var provider = result.Properties.Items["scheme"];
			var providerUserId = userIdClaim.Value;

			var user = await _userManager.FindByLoginAsync(provider, providerUserId);
			user ??= await AutoProvisionUserAsync(provider, providerUserId, externalUser.Claims);

			var additionalLocalClaims = new List<Claim>();
			var localSignInProps = new AuthenticationProperties();
			CaptureExternalLoginContext(result, additionalLocalClaims, localSignInProps);

			await _signInManager.SignInWithClaimsAsync(user, localSignInProps, additionalLocalClaims);
			await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

			var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

			var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
			await _events.RaiseAsync(new UserLoginSuccessEvent(
				provider,
				providerUserId,
				user.Id.ToString(),
				user.UserName,
				true,
				context?.Client.ClientId));

			return Redirect(returnUrl);
		}

		private async Task<ApplicationUser> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
		{
			var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
			var user = new ApplicationUser(email)
			{
				FirstName = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value,
				LastName = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value
			};

			var identityResult = await _userManager.CreateAsync(user);
			if (!identityResult.Succeeded)
			{
				throw new Exception(identityResult.Errors.First().Description);
			}

			identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
			if (!identityResult.Succeeded)
			{
				throw new Exception(identityResult.Errors.First().Description);
			}

			return user;
		}

		private void CaptureExternalLoginContext(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
		{
			localClaims.Add(new Claim(JwtClaimTypes.IdentityProvider, externalResult.Properties.Items["scheme"]));

			var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
			if (sid != null)
			{
				localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
			}

			var idToken = externalResult.Properties.GetTokenValue("id_token");
			if (idToken != null)
			{
				localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
			}
		}
	}
}