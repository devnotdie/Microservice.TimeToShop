using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Threading.Tasks;

namespace BuildingBlocks.Grpc.Interceptors
{
	public class LoggingInterceptor : Interceptor
	{
		private readonly ILogger<LoggingInterceptor> _logger;

		public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
		{
			_logger = logger;
		}

		public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
			TRequest request,
			ServerCallContext context,
			UnaryServerMethod<TRequest, TResponse> continuation)
		{
			using (LogContext.PushProperty("RequestBody", request, true))
			{
				var requestName = typeof(TRequest).Name;
				_logger.LogInformation("Grpc request {RequestName}", requestName);
			}

			var response = await continuation(request, context);

			using (LogContext.PushProperty("ResponseBody", response, true))
			{
				var responseName = typeof(TResponse).Name;
				_logger.LogInformation("Grpc response {ResponseName}", responseName);
			}

			return response;
		}
	}
}
