using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Common.Models.Result
{
	public interface IResult
	{
		bool Successful { get; }

		List<string> ErrorMessages { get; }

		ResultErrorType ErrorType { get; }
	}

	public class ResultBase<T> : IResult where T : ResultBase<T>, new()
	{
		public bool Successful { get; private init; }

		public List<string> ErrorMessages { get; } = [];

		public ResultErrorType ErrorType { get; private init; }

		public static T Success()
		{
			return new T { Successful = true };
		}

		public static T Failure(string errorMessage)
		{
			return Failure(ResultErrorType.Common, new List<string> { errorMessage });
		}

		public static T Failure(ResultErrorType errorType)
		{
			return new T { ErrorType = errorType };
		}

		public static T Failure(ResultErrorType errorType, string errorMessage)
		{
			return Failure(errorType, new List<string> { errorMessage });
		}

		public static T Failure(ICollection<string> errorMessages)
		{
			var result = Failure(ResultErrorType.Common);
			result.ErrorMessages.AddRange(errorMessages);
			return result;
		}

		public static T Failure(ResultErrorType errorType, ICollection<string> errorMessages)
		{
			var result = Failure(errorType);
			result.ErrorMessages.AddRange(errorMessages);
			return result;
		}

		public static T Failure(IResult result)
		{
			return Failure(result.ErrorType, result.ErrorMessages);
		}

		public static T Failure(ValidationResult validationResult)
		{
			throw new NotImplementedException();
		}
	}

	public enum ResultErrorType
	{
		Common = 0,

		NotFound = 1,

		Access = 2,

		Validation = 3
	}
}
