using System;

namespace Fake.Domain
{
	public class User
	{
		public Guid Id { get; set; }

		public string Email { get; set; }

		public string Password { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public DateTime CreatedOn { get; set; }

		public DateTime? ModifiedOn { get; set; }
	}
}
