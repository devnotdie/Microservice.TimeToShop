using BuildingBlocks.Common.Errors;
using FluentResults;
using Grpc.Core;
using System;
using System.Text;

namespace BuildingBlocks.Grpc.Extensions
{
	public static class GrpcStatusExtensions
	{
		public static Status ToGrpcFailResult<T>(this Result<T> result)
		{
			ArgumentNullException.ThrowIfNull(result);

			var error = result.Errors[0];
			var message = ConvertToSingleMessage(error);

			if (error is ValidationError)
			{
				return new Status(StatusCode.InvalidArgument, message);
			}

			return new Status(StatusCode.Internal, message);
		}

		private static string ConvertToSingleMessage(IError error)
		{
			var sb = new StringBuilder();
			sb.Append($"Message: {error.Message}; ");

			if(error.Reasons.Count > 0)
			{
				sb.Append("Reasons: ");
				sb.Append(string.Join(", ", error.Reasons));
			}

			return sb.ToString();
		}
	}
}
