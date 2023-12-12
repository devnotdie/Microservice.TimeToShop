using AutoMapper;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Identity.API.Models;
using Identity.API.Pages.Account.Shared.Models;
using Identity.API.Services.ExternalProviders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.API.Pages.Login
{
	[SecurityHeaders]
	[AllowAnonymous]
	public class Index : PageModel
	{
		private readonly IMapper _mapper;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IIdentityServerInteractionService _interaction;
		private readonly IEventService _events;
		private readonly IExternalProviderService _externalProviderService;

		public ViewModel View { get; set; }

		[BindProperty]
		public InputModel Input { get; set; }

		public Index(
			IMapper mapper,
			IIdentityServerInteractionService interaction,
			IEventService events,
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IExternalProviderService externalProviderService)
		{
			_mapper = mapper;
			_userManager = userManager;
			_signInManager = signInManager;
			_interaction = interaction;
			_events = events;
			_externalProviderService = externalProviderService;
		}

		public async Task<IActionResult> OnGet(string returnUrl)
		{
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				return Redirect("/");
			}

			await BuildModelAsync(returnUrl);

			return Page();
		}

		public async Task<IActionResult> OnPost()
		{
			if (ModelState.IsValid)
			{
				var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

				var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, true, lockoutOnFailure: true);
				if (result.Succeeded)
				{
					var user = await _userManager.FindByNameAsync(Input.Email);
					await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName, clientId: context?.Client.ClientId));

					if (context != null)
					{
						return Redirect(Input.ReturnUrl);
					}

					if (Url.IsLocalUrl(Input.ReturnUrl))
					{
						return Redirect(Input.ReturnUrl);
					}
					else if (string.IsNullOrEmpty(Input.ReturnUrl))
					{
						return Redirect("~/");
					}
					else
					{
						throw new Exception("invalid return URL");
					}
				}

				await _events.RaiseAsync(new UserLoginFailureEvent(Input.Email, "invalid credentials", clientId: context?.Client.ClientId));
				ModelState.AddModelError(string.Empty, LoginOptions.InvalidCredentialsErrorMessage);
			}

			await BuildModelAsync(Input.ReturnUrl);
			return Page();
		}

		private async Task BuildModelAsync(string returnUrl)
		{
			Input = new InputModel
			{
				ReturnUrl = returnUrl
			};

			var providers = await _externalProviderService.GetAllAsync();

			View = new ViewModel
			{
				ExternalProviders = _mapper.Map<ICollection<ExternalProviderViewModel>>(providers)
			};
		}
	}
}