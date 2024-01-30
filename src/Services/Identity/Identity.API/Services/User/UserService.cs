using AutoMapper;
using BuildingBlocks.Common.Errors;
using FluentResults;
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
		Task<Result<UserModel>> AddUserAsync(CreateUserModel userModel);

		Task<Result<UserModel>> AddUserAsync(CreateUserModel createUser, HashSet<string> roleNames);

		Task<Result<UserModel>> GetUserByIdAsync(Guid id);
	}

	public class UserService : IUserService
	{
		private readonly IMapper _mapper;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<ApplicationRole> _roleManager;

		public UserService(
			IMapper mapper,
			UserManager<ApplicationUser> userManager,
			RoleManager<ApplicationRole> roleManager)
		{
			_mapper = mapper;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public Task<Result<UserModel>> AddUserAsync(CreateUserModel userModel)
		{
			ArgumentNullException.ThrowIfNull(userModel);

			return AddUserAsync(userModel, []);
		}

		public async Task<Result<UserModel>> AddUserAsync(CreateUserModel userModel, HashSet<string> roleNames)
		{
			ArgumentNullException.ThrowIfNull(userModel);
			ArgumentNullException.ThrowIfNull(roleNames);

			var result = await CreateRoles(roleNames);
			if (result.IsFailed)
			{
				return result;
			}

			var user = await _userManager.FindByEmailAsync(userModel.Email);
			if (user != null)
			{
				return Result.Fail(new ValidationError("User already exists"));
			}

			user = new ApplicationUser(userModel.Email)
			{
				FirstName = userModel.FirstName,
				LastName = userModel.LastName,
				EmailConfirmed = true
			};

			var identityResult = await _userManager.CreateAsync(user, userModel.Password);
			if (!identityResult.Succeeded)
			{
				return identityResult.ToFailResult("User cannot be added");
			}

			foreach (var roleName in roleNames)
			{
				identityResult = await _userManager.AddToRoleAsync(user, roleName);
				if (!identityResult.Succeeded)
				{
					return identityResult.ToFailResult("Role cannot be added");
				}
			}

			foreach (var roleName in roleNames)
			{
				identityResult = await _userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Role, roleName));
				if (!identityResult.Succeeded)
				{
					return identityResult.ToFailResult("Claim cannot be added");
				}
			}

			return _mapper.Map<UserModel>(user).ToResult();
		}

		public async Task<Result<UserModel>> GetUserByIdAsync(Guid id)
		{
			var user = await _userManager.FindByIdAsync(id.ToString());
			if (user == null)
			{
				return Result.Fail("User not found");
			}

			return _mapper.Map<UserModel>(user).ToResult();
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
					return result.ToFailResult("Role cannot be created");
				}
			}

			return Result.Ok();
		}
	}
}
