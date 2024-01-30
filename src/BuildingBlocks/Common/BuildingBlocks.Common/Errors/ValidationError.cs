using FluentResults;

namespace BuildingBlocks.Common.Errors
{
	public class ValidationError : Error
	{
		protected ValidationError() { }

		public ValidationError(string message) : base(message) { }

		public ValidationError(string message, IError causedBy) : base(message, causedBy) { }
	}
}
