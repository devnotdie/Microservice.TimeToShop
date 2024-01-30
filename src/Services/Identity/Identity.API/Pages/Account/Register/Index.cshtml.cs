using AutoMapper;
using Identity.API.Pages.Account.Shared.Models;
using Identity.API.Services.ExternalProviders;
using Identity.API.Services.User;
using Identity.API.Services.User.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Pages.Account.Register
{
	public class Index : PageModel
	{
		private readonly IMapper _mapper;
		private readonly IUserService _userService;
		private readonly IExternalProviderService _externalProviderService;

		public Index(
			IMapper mapper,
			IUserService userService,
			IExternalProviderService externalProviderService)
		{
			_mapper = mapper;
			_userService = userService;
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
				var result = await _userService.AddUserAsync(_mapper.Map<CreateUserModel>(Input));
				if (result.IsFailed)
				{
					foreach (var reasons in result.Errors.SelectMany(e => e.Reasons))
					{
						ModelState.AddModelError("registration", reasons.Message);
					}

					await BuildModelAsync(Input.ReturnUrl);
					return Page();
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
