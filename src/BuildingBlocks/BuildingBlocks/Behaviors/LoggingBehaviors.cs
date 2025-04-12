using System.Diagnostics;

using MediatR;

using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Behaviors;

public class LoggingBehaviors<TRequest, TResponse> (
    ILogger<LoggingBehaviors<TRequest , TResponse>> logger )
    : IPipelineBehavior<TRequest , TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    private readonly ILogger<LoggingBehaviors<TRequest , TResponse>> _logger = logger;

    public async Task<TResponse> Handle (
        TRequest request ,
        RequestHandlerDelegate<TResponse> next ,
        CancellationToken cancellationToken )
    {
        _logger.LogInformation (
            "[START] Handle request={Request} - Response={Response} - RequestData={RequestData}" ,
            typeof ( TRequest ).Name ,
            typeof ( TResponse ).Name ,
            request );

        var timer = Stopwatch.StartNew ( );
        try
        {
            return await next ( );
        }
        finally
        {
            timer.Stop ( );
            var timeTaken = timer.Elapsed;

            _logger.LogInformation (
                "[END] Handle request={Request} - Response={Response} - TimeTaken={TimeTaken}ms" ,
                typeof ( TRequest ).Name ,
                typeof ( TResponse ).Name ,
                timeTaken.TotalMilliseconds );

            if ( timeTaken.TotalMilliseconds > 1000 )
            {
                _logger.LogWarning (
                    "[WARNING] Handle request={Request} - Response={Response} - TimeTaken={TimeTaken}ms" ,
                    typeof ( TRequest ).Name ,
                    typeof ( TResponse ).Name ,
                    timeTaken.TotalMilliseconds );
            }
        }
    }
}