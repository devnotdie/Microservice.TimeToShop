using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Models;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.API.Profiles
{
	public class CommonProfileService : ProfileService<ApplicationUser>
	{
		public CommonProfileService(
			UserManager<ApplicationUser> userManager, 
			IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, 
			ILogger<ProfileService<ApplicationUser>> logger) : base (userManager, claimsFactory, logger)
		{
		}
		protected override Task GetProfileDataAsync(ProfileDataRequestContext context, ApplicationUser user)
		{
			var s = context.RequestedClaimTypes.ToList();

			return base.GetProfileDataAsync(context, user);
		}
		protected override async Task<ClaimsPrincipal> GetUserClaimsAsync(ApplicationUser user)
		{
			var principal = await ClaimsFactory.CreateAsync(user);
			if (principal == null)
				throw new Exception("ClaimsFactory failed to create a principal");

			return principal;
		}
	}
}
