using Microsoft.AspNetCore.Mvc.Filters;

namespace IotControlService.Filters;

public class RequestResponseLoggingFilter : IAsyncActionFilter
{
    private readonly ILogger<RequestResponseLoggingFilter> _logger;

    public RequestResponseLoggingFilter(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<RequestResponseLoggingFilter>();
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        LogRequest(request);
        await next();
        var response = context.HttpContext.Response;
        LogResponse(response);
    }

    private void LogRequest(HttpRequest request)
    {
        _logger.LogInformation("[Incoming Request]");
        _logger.LogInformation("URL: {Method} {Path}", request.Method, request.Path);
        _logger.LogInformation("Headers: {Headers}", string.Join(", ", request.Headers.Select(h => $"{h.Key}: {h.Value}")));
    }

    private void LogResponse(HttpResponse response)
    {
        _logger.LogInformation("[Response]");
        _logger.LogInformation("Status Code: {StatusCode}", response.StatusCode);
    }
}