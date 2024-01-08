namespace BuildingBlocks.Common.Models.Result
{
	public class Result : ResultBase<Result>
	{
	}

	public class Result<T> : ResultBase<Result<T>>
	{
		public T Data { get; set; }

		public static Result<T> Success(T data)
		{
			var result = Success();
			result.Data = data;
			return result;
		}
	}
}
