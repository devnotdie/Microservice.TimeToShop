using FluentResults;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace Identity.API.Extensions
{
	public static class IdentityResultExtensions
	{
		public static Result ToFailResult(this IdentityResult identityResult, string errorMessage)
		{
			var error = new Error(errorMessage);
			error.CausedBy(identityResult.Errors.Select(e => e.Description));
			return error;
		}
	}
}
