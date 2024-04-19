using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation(
            "Start request {RequestName}", requestName);

        var result = await next();

        if (result.IsSuccess)
            logger.LogInformation(
                "Completed request {RequestName}", requestName);
        else
            logger.LogError(
                "Request {RequestName} failed with error with {@Error}", requestName, result.Errors);

        return result;
    }
}