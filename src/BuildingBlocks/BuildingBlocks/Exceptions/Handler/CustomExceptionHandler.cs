using FluentValidation;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Exceptions.Handler;

public class CustomExceptionHandler ( ILogger<CustomExceptionHandler> logger ) : IExceptionHandler
{
    private readonly ILogger<CustomExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync ( HttpContext httpContext , Exception exception , CancellationToken cancellationToken )
    {
        _logger.LogError ( exception , "[Error Message]: ({Message}), Time of occurrence [{time}]" , exception.Message , DateTime.UtcNow );

        // Use Pattern Matching to determine the type of exception
        (string Detail, string Title, int StatusCode) = exception switch
        {
            InternalServerException => (exception.Message, exception.GetType ( ).Name, httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError),
            ValidationException => (exception.Message, exception.GetType ( ).Name, httpContext.Response.StatusCode = StatusCodes.Status400BadRequest),
            BadRequestException => (exception.Message, exception.GetType ( ).Name, httpContext.Response.StatusCode = StatusCodes.Status400BadRequest),
            NotFoundException => (exception.Message, exception.GetType ( ).Name, httpContext.Response.StatusCode = StatusCodes.Status404NotFound),
            _ => (exception.Message, exception.GetType ( ).Name, httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError)
        };

        var problemDetails = new ProblemDetails
        {
            Title = Title ,
            Detail = Detail ,
            Status = StatusCode ,
            Type = exception.GetType ( ).Name
        };

        problemDetails.Extensions.Add ( "traceId" , httpContext.TraceIdentifier );

        if ( exception is ValidationException validationException )
        {
            // FluentValidation's ValidationException has the Errors property
            problemDetails.Extensions.Add ( "ValidationErrors" , validationException.Errors );
        }

        await httpContext.Response.WriteAsJsonAsync ( problemDetails , cancellationToken: cancellationToken );

        return true;
    }
}

