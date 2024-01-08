namespace Identity.API.Services.User.Models
{
	public class CreateUser
	{
		public string Email { get; set; }

		public string Password { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }
	}
}
