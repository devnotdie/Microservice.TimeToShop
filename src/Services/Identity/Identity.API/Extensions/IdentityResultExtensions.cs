using BuildingBlocks.Common.Models.Result;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace Identity.API.Extensions
{
	public static class IdentityResultExtensions
	{
		public static Result<T> ToFailureResult<T>(this IdentityResult identityResult)
		{
			return Result<T>.Failure(GetIdentityDescriptions(identityResult.Errors));
		}

		public static Result ToFailureResult(this IdentityResult identityResult)
		{
			return Result.Failure(GetIdentityDescriptions(identityResult.Errors));
		}

		private static ICollection<string> GetIdentityDescriptions(IEnumerable<IdentityError> identityErrors)
		{
			return identityErrors.Select(e => e.Description).ToArray();
		}
	}
}
