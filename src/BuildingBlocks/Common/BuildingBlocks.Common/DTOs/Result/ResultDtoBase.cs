using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Common.DTOs.Result
{
	public interface IResultDto
	{
		bool Successful { get; }

		public string Message { get; }

		public IEnumerable<string> DescriptionErrors { get; }
	}

	public class ResultDtoBase<T> : IResultDto where T : ResultDtoBase<T>, new()
	{
		public bool Successful { get; private init; }

		public string Message { get; private init; }

		public IEnumerable<string> DescriptionErrors { get; private init; } = [];

		public static T Success()
		{
			return new T { Successful = true };
		}

		public static T Failure()
		{
			return new T();
		}

		public static T Failure(string errorMessage)
		{
			return new T
			{
				Message = errorMessage
			};
		}

		public static T Failure(string errorMessage, IEnumerable<string> descriptionErrors)
		{
			return new T
			{
				Message = errorMessage,
				DescriptionErrors = descriptionErrors ?? Array.Empty<string>()
			};
		}

		public static T Failure(IResultDto result)
		{
			return Failure(result.Message, result.DescriptionErrors);
		}

		public static T Failure(ValidationResult validationResult)
		{
			throw new NotImplementedException();
		}
	}
}
