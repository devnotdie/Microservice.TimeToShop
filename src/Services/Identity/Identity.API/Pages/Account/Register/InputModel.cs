using System.ComponentModel.DataAnnotations;

namespace Identity.API.Pages.Account.Register
{
	public class InputModel
	{
		[Required]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		[Compare(nameof(Password))]
		public string ConfirmPassword { get; set; }

		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		public string ReturnUrl { get; set; }
	}
}
