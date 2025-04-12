
using BuildingBlocks.CQRS.Requests;

using FluentValidation;

using MediatR;

namespace BuildingBlocks.Behaviors;

public class ValidationBehaviors<TRequest, TResponse> ( IEnumerable<IValidator<TRequest>> validators ) : IPipelineBehavior<TRequest , TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;
    public async Task<TResponse> Handle ( TRequest request , RequestHandlerDelegate<TResponse> next , CancellationToken cancellationToken )
    {
        var context = new ValidationContext<TRequest> ( request );

        var validationResults = await Task.WhenAll ( _validators.Select ( x => x.ValidateAsync ( context , cancellationToken ) ) );

        var failures = validationResults
            .SelectMany ( x => x.Errors )
            .Where ( x => x is not null )
            .ToList ( );

        if ( failures.Count != 0 )
        {
            throw new ValidationException ( failures );
        }

        return await next ( );
    }
}
