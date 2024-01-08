using Grpc.Core;
using Identity.API.Models;
using Identity.API.Protos;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Identity.API.Grpc
{
	public class UserGrpcService : UserGrpc.UserGrpcBase
	{
		//private readonly UserManager<ApplicationUser> _userManager;

		//public UserGrpcService(UserManager<ApplicationUser> userManager)
		//{
		//	_userManager = userManager;
		//}

		public override Task<UserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
		{
			return base.CreateUser(request, context);
		}

		public override async Task<UserResponse> GetUser(UserRequest request, ServerCallContext context)
		{
			//var user = await _userManager.FindByIdAsync(request.Id);
			//if (user != null)
			//{
			//	return new UserResponse
			//	{
			//		Id = user.Id.ToString(),
			//		Email = user.Email,
			//		FirstName = user.FirstName,
			//		LastName = user.LastName
			//	};
			//}

			context.Status = new Status(StatusCode.NotFound, "User not found");
			return new UserResponse();
		}
	}
}
