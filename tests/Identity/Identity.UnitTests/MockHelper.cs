using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;

namespace Identity.UnitTests
{
	internal static class MockHelper
	{
		public static Mock<UserManager<ApplicationUser>> GetMockUserManager()
		{
			return new Mock<UserManager<ApplicationUser>>(
				new Mock<IUserStore<ApplicationUser>>().Object,
				new Mock<IOptions<IdentityOptions>>().Object,
				new Mock<IPasswordHasher<ApplicationUser>>().Object,
				new IUserValidator<ApplicationUser>[0],
				new IPasswordValidator<ApplicationUser>[0],
				new Mock<ILookupNormalizer>().Object,
				new Mock<IdentityErrorDescriber>().Object,
				new Mock<IServiceProvider>().Object,
				new Mock<ILogger<UserManager<ApplicationUser>>>().Object);
		}
	}
}
