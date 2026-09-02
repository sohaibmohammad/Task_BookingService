using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace BackendBookingManagement.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

		var (statusCode, message) = exception switch
		{
			KeyNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
			InvalidOperationException => (StatusCodes.Status400BadRequest, exception.Message),
			ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
			DbUpdateException => (StatusCodes.Status409Conflict, "A database error occurred or a conflict was detected."),
			_ => (StatusCodes.Status500InternalServerError, "An internal server error occurred.")
		};

		httpContext.Response.StatusCode = statusCode;

		var response = new
		{
			status = statusCode,
			message = message
		};

		await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

 		return true;
	}
}
