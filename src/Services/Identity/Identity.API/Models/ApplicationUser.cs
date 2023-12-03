using Microsoft.AspNetCore.Identity;
using System;

namespace Identity.API.Models
{
	public class ApplicationUser : IdentityUser<Guid>
	{
		public ApplicationUser() { }

		public ApplicationUser(string userName) : base(userName)
		{
			Email = userName;
		}
	}
}
