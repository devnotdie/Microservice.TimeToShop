using System;

namespace Identity.API.Services.User.Models
{
	public class UserModel
	{
		public Guid Id { get; set; }

		public string Email { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }
	}
}
