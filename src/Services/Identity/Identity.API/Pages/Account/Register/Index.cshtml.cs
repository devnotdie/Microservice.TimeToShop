using AutoMapper;
using Identity.API.Models;
using Identity.API.Pages.Account.Shared.Models;
using Identity.API.Services.ExternalProviders;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Pages.Account.Register
{
	public class Index : PageModel
	{
		private readonly IMapper _mapper;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IExternalProviderService _externalProviderService;

		public Index(
			IMapper mapper,
			UserManager<ApplicationUser> userManager,
			IExternalProviderService externalProviderService)
		{
			_mapper = mapper;
			_userManager = userManager;
			_externalProviderService = externalProviderService;
		}

		public ViewModel View { get; set; }

		[BindProperty]
		public InputModel Input { get; set; }

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
				var user = await _userManager.FindByNameAsync(Input.Email);
				if (user != null)
				{
					ModelState.AddModelError("registration", "Email already exists");
					await BuildModelAsync(Input.ReturnUrl);
					return Page();
				}

				user = new ApplicationUser(Input.Email)
				{
					FirstName = Input.FirstName,
					LastName = Input.LastName,
					EmailConfirmed = true
				};

				var result = await _userManager.CreateAsync(user, Input.Password);
				if (!result.Succeeded)
				{
					throw new Exception(result.Errors.First().Description);
				}

				return string.IsNullOrWhiteSpace(Input.ReturnUrl)
					? Redirect("~/")
					: Redirect(Input.ReturnUrl);
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
