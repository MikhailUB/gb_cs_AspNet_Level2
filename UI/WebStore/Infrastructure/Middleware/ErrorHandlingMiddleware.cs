using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WebStore.Infrastructure.Middleware
{
	public class ErrorHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ErrorHandlingMiddleware> _logger;

		public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception error)
			{
				await HandleExceptionAsync(context, error);
				throw;
			}
		}

		private Task HandleExceptionAsync(HttpContext context, Exception error)
		{
			_logger.LogError(error, "В ходе обработки входящего запроса возникло исключение");
			return Task.CompletedTask;
		}
	}
}
