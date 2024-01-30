using AutoMapper;
using FluentAssertions;
using Identity.API.Models;
using Identity.API.Services.User;
using Identity.API.Services.User.Models;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Identity.UnitTests
{
	public class UserServiceTests
	{

		[Fact]
		public async Task AddUserAsync_ShouldReturnError_WhenUserModelIsNull()
		{
			// Arrange
			var userService = new UserService(null, null, null);

			// Act
			Func<Task> act = async () => await userService.AddUserAsync(null, []);

			// Assert
			await act.Should().ThrowAsync<ArgumentNullException>();
		}

		[Fact]
		public async Task AddUserAsync_ShouldReturnError_WhenRoleNamesIsNull()
		{
			// Arrange
			var userService = new UserService(null, null, null);

			// Act
			Func<Task> act = async () => await userService.AddUserAsync(new CreateUserModel(), null);

			// Assert
			await act.Should().ThrowAsync<ArgumentNullException>();
		}

		[Fact]
		public async Task AddUserAsync_ShouldReturnError_WhenUserAlreadyExists()
		{
			// Arrange
			var email = "test@test.com";

			var mockUserManager = MockHelper.GetMockUserManager();
			mockUserManager
				.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync(new ApplicationUser(email));

			var userService = new UserService(new Mock<IMapper>().Object, mockUserManager.Object, null);
			var userModel = new CreateUserModel { Email = email };

			// Act
			var result = await userService.AddUserAsync(userModel, []);

			// Assert
			result.IsFailed.Should().BeTrue();
			result.Errors.Should().NotBeEmpty();
			mockUserManager.Verify(x => x.FindByEmailAsync(userModel.Email), Times.Once);
		}

		[Fact]
		public async Task AddUserAsync_ShouldReturnError_WhenCreateUser()
		{
			// Arrange
			var email = "test@test.com";
			var password = "password";

			var mockUserManager = MockHelper.GetMockUserManager();
			mockUserManager
				.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser)null);

			mockUserManager
				.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Failed(new IdentityError()));

			var userService = new UserService(new Mock<IMapper>().Object, mockUserManager.Object, null);
			var userModel = new CreateUserModel { Email = email, Password = password };

			// Act
			var result = await userService.AddUserAsync(userModel, []);

			// Assert
			result.IsFailed.Should().BeTrue();
			result.Errors.Should().NotBeEmpty();
			mockUserManager.Verify(x => x.FindByEmailAsync(email), Times.Once);
		}

		[Fact]
		public async Task AddUserAsync_ShouldSuccess_WhenCreateUserWithoutRoles()
		{
			// Arrange
			var createUserModel = new CreateUserModel
			{
				Email = "test@test.com",
				Password = "password",
				FirstName = "FirstName",
				LastName = "LastName"
			};

			var mockUserManager = MockHelper.GetMockUserManager();
			mockUserManager
				.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser)null);

			mockUserManager
				.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Success);

			var mockMapper = new Mock<IMapper>();
			mockMapper
				.Setup(x => x.Map<UserModel>(It.IsAny<ApplicationUser>()))
				.Returns(new UserModel
				{
					Id = Guid.NewGuid(),
					Email = createUserModel.Email,
					FirstName = createUserModel.FirstName,
					LastName = createUserModel.LastName
				});

			var userService = new UserService(mockMapper.Object, mockUserManager.Object, null);

			// Act
			var result = await userService.AddUserAsync(createUserModel, []);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.ValueOrDefault.Should().NotBeNull();
			result.ValueOrDefault.Id.Should().NotBeEmpty();
			result.ValueOrDefault.Email.Should().NotBeEmpty();
			result.ValueOrDefault.FirstName.Should().NotBeEmpty();
			result.ValueOrDefault.LastName.Should().NotBeEmpty();
			result.Errors.Should().BeEmpty();
			mockUserManager.Verify(x => x.FindByEmailAsync(createUserModel.Email), Times.Once);
		}
	}
}