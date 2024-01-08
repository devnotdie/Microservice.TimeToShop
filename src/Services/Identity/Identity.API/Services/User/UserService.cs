using BuildingBlocks.Common.Models.Result;
using Identity.API.Extensions;
using Identity.API.Models;
using Identity.API.Services.User.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.API.Services.User
{
	public interface IUserService
	{
		Task<Result<ApplicationUser>> AddUserAsync(CreateUser createUser, HashSet<string> roleNames);
	}

	public class UserService : IUserService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<ApplicationRole> _roleManager;

		public UserService(
			UserManager<ApplicationUser> userManager,
			RoleManager<ApplicationRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public async Task<Result<ApplicationUser>> AddUserAsync(CreateUser createUser, HashSet<string> roleNames)
		{
			ArgumentNullException.ThrowIfNull(createUser);
			ArgumentNullException.ThrowIfNull(roleNames);

			var result = await CreateRoles(roleNames);
			if (!result.Successful)
			{
				return Result<ApplicationUser>.Failure(result);
			}

			var user = await _userManager.FindByEmailAsync(createUser.Email);
			if (user != null)
			{
				throw new Exception("User already exists");
			}

			user = new ApplicationUser(createUser.Email)
			{
				FirstName = createUser.FirstName,
				LastName = createUser.LastName,
				EmailConfirmed = true
			};

			var identityResult = await _userManager.CreateAsync(user, createUser.Password);
			if (!identityResult.Succeeded)
			{
				return identityResult.ToFailureResult<ApplicationUser>();
			}

			foreach (var roleName in roleNames)
			{
				identityResult = await _userManager.AddToRoleAsync(user, roleName);
				if (!identityResult.Succeeded)
				{
					return identityResult.ToFailureResult<ApplicationUser>();
				}
			}

			foreach (var roleName in roleNames)
			{
				identityResult = await _userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Role, roleName));
				if (!identityResult.Succeeded)
				{
					return identityResult.ToFailureResult<ApplicationUser>();
				}
			}

			return Result<ApplicationUser>.Success(user);
		}

		private async Task<Result> CreateRoles(HashSet<string> roleNames)
		{
			ArgumentNullException.ThrowIfNull(roleNames);

			foreach (var roleName in roleNames)
			{
				var role = await _roleManager.FindByNameAsync(roleName);
				if (role != null)
				{
					continue;
				}

				role = new ApplicationRole(roleName);
				var result = await _roleManager.CreateAsync(role);
				if (!result.Succeeded)
				{
					return result.ToFailureResult();
				}
			}

			return Result.Success();
		}
	}
}
