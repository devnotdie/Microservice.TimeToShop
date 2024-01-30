using BuildingBlocks.Grpc.Extensions;
using Grpc.Core;
using Identity.API.Protos;
using Identity.API.Services.User;
using Identity.API.Services.User.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace Identity.API.Grpc
{
	[Authorize(AuthenticationSchemes ="Bearer", Roles = "Admin")]
	public class UserGrpcService : UserGrpc.UserGrpcBase
	{
		private readonly IUserService _userService;

		public UserGrpcService(IUserService userService)
		{
			_userService = userService;
		}

		public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
		{
			var result = await _userService.AddUserAsync(new CreateUserModel
			{
				Email = request.Email,
				Password = request.Password,
				FirstName = request.FirstName,
				LastName = request.LastName,
			});

			if (result.IsFailed)
			{
				context.Status = result.ToGrpcFailResult();
				return new CreateUserResponse();
			}

			return new CreateUserResponse
			{
				Id = result.Value.Id.ToString()
			};
		}

		public override async Task<UserResponse> GetUser(UserRequest request, ServerCallContext context)
		{
			var result = await _userService.GetUserByIdAsync(Guid.Parse(request.Id));
			if (result.IsFailed)
			{
				context.Status = result.ToGrpcFailResult();
				return new UserResponse();
			}

			return new UserResponse
			{
				Id = result.Value.Id.ToString(),
				Email = result.Value.Email,
				FirstName = result.Value.FirstName,
				LastName = result.Value.LastName
			};
		}
	}
}
