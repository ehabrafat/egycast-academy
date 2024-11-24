using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EgycastApi;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var ex = (EgycastException)exception;
        var problemDetails = new ProblemDetails
        {
            Status = ex.Status,
            Extensions = new Dictionary<string, object?>
            {
                {"error", ex.Message}
            }
        };
        httpContext.Response.StatusCode = ex.Status;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}