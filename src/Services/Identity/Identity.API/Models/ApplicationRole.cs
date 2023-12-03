using Microsoft.AspNetCore.Identity;
using System;

namespace Identity.API.Models
{
	public class ApplicationRole : IdentityRole<Guid>
	{
		public ApplicationRole() { }

		public ApplicationRole(string roleName) : base(roleName) { }
	}
}
