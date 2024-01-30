using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BuildingBlocks.Common.Grpc.Interceptors
{
	public class ExceptionInterceptor : Interceptor
	{
		private readonly ILogger<ExceptionInterceptor> _logger;

		public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)
		{
			_logger = logger;
		}

		public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
			TRequest request,
			ClientInterceptorContext<TRequest, TResponse> context,
			AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
		{
			try
			{
				return continuation(request, context);
			}
			catch (Exception exception)
			{
				throw HandleRpcException(exception);
			}
		}

		public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
			TRequest request,
			ServerCallContext context,
			UnaryServerMethod<TRequest, TResponse> continuation)
		{
			try
			{
				return await continuation(request, context);
			}
			catch (Exception exception)
			{
				throw HandleRpcException(exception);
			}
		}

		public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
			IAsyncStreamReader<TRequest> requestStream,
			ServerCallContext context,
			ClientStreamingServerMethod<TRequest, TResponse> continuation)
		{
			try
			{
				return await continuation(requestStream, context);
			}
			catch (Exception exception)
			{
				throw HandleRpcException(exception);
			}
		}


		public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
			TRequest request,
			IServerStreamWriter<TResponse> responseStream,
			ServerCallContext context,
			ServerStreamingServerMethod<TRequest, TResponse> continuation)
		{
			try
			{
				await continuation(request, responseStream, context);
			}
			catch (Exception exception)
			{
				throw HandleRpcException(exception);
			}
		}

		public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
			IAsyncStreamReader<TRequest> requestStream,
			IServerStreamWriter<TResponse> responseStream,
			ServerCallContext context,
			DuplexStreamingServerMethod<TRequest, TResponse> continuation)
		{
			try
			{
				await continuation(requestStream, responseStream, context);
			}
			catch (Exception exception)
			{
				throw HandleRpcException(exception);
			}
		}

		private RpcException HandleRpcException(Exception exception)
		{
			var correlationId = Guid.NewGuid();
			_logger.LogError(exception, $"CorrelationId: {correlationId} - An error occurred");
			return new RpcException(new Status(StatusCode.Internal, exception.Message), CreateTrailers(correlationId));
		}

		private static Metadata CreateTrailers(Guid correlationId)
		{
			var trailers = new Metadata
			{
				{ "CorrelationId", correlationId.ToString() }
			};
			return trailers;
		}
	}
}
