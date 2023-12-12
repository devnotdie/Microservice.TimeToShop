using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;

namespace Identity.API.Pages.Home
{
	[AllowAnonymous]
	public class Index : PageModel
	{
		public bool IsAuthenticated;

		public void OnGet()
		{
			IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
		}
	}
}