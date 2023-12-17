using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Identity.API.Models
{
	public class ApplicationUser : IdentityUser<Guid>
	{
		public ApplicationUser() { }

		public ApplicationUser(string userName) : base(userName)
		{
			Email = userName;
		}

		[MaxLength(100)]
		public string FirstName { get; set; }

		[MaxLength(100)]
		public string LastName { get; set; }
	}
}
