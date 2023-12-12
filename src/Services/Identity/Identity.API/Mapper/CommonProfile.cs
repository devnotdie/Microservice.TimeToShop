using AutoMapper;
using Identity.API.Pages.Account.Shared.Models;
using Identity.API.Services.ExternalProviders.Models;

namespace Identity.API.Mapper
{
	public class CommonProfile : Profile
	{
		public CommonProfile()
		{
			CreateMap<ExternalProviderModel, ExternalProviderViewModel>();
		}
	}
}
