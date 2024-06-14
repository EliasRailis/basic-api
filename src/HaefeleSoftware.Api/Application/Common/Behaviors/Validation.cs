using System.Net;
using FluentValidation;
using FluentValidation.Results;
using HaefeleSoftware.Api.Domain.Types;
using MediatR;

namespace HaefeleSoftware.Api.Application.Common.Behaviors;

public sealed class Validation<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IValidator<TRequest>? _validator;

    public Validation(IValidator<TRequest>? validator)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        if (_validator is null) return await next();
        
        ValidationResult result = await _validator.ValidateAsync(request, cancellationToken);
        
        if (result.IsValid) return await next();

        OnError errors = new OnError(HttpStatusCode.BadRequest, result.Errors);
        return (dynamic) errors;
    }
}