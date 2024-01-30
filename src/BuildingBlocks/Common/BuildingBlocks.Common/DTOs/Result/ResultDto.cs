namespace BuildingBlocks.Common.DTOs.Result
{
	public class ResultDto : ResultDtoBase<ResultDto>
	{
	}

	public class ResultDto<T> : ResultDtoBase<ResultDto<T>>
	{
		public T Data { get; set; }

		public static ResultDto<T> Success(T data)
		{
			var result = Success();
			result.Data = data;
			return result;
		}
	}
}
