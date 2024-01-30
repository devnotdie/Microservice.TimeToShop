using AutoMapper;
using Identity.API.Models;
using Identity.API.Pages.Account.Shared.Models;
using Identity.API.Services.ExternalProviders.Models;
using Identity.API.Services.User.Models;

namespace Identity.API.Mapper
{
	public class CommonProfile : Profile
	{
		public CommonProfile()
		{
			CreateMap<ExternalProviderModel, ExternalProviderViewModel>();
			CreateMap<ApplicationUser, UserModel>();
			CreateMap<Pages.Account.Register.InputModel, CreateUserModel>();
		}
	}
}
