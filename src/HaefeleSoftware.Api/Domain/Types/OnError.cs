using System.Net;
using FluentValidation.Results;
using Newtonsoft.Json;

namespace HaefeleSoftware.Api.Domain.Types;

public sealed class ErrorBody
{
    public string Property { get; init; } = string.Empty;
    
    public string Message { get; init; } = string.Empty;

    public object Value { get; init; } = string.Empty;
}

public readonly struct OnError
{
    public HttpStatusCode StatusCode { get; init;  } = HttpStatusCode.BadRequest;
    
    public DateTime RequestTime => DateTime.UtcNow;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? Error { get; init; } = string.Empty;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public IEnumerable<ErrorBody> Errors { get; init; } = [];

    public OnError(HttpStatusCode statusCode, IEnumerable<ValidationFailure> errors)
    {
        StatusCode = statusCode;
        Error = "Validation failed with multiple errors.";
        Errors = errors.Select(error => new ErrorBody
        {
            Property = error.PropertyName,
            Message = error.ErrorMessage,
            Value = error.AttemptedValue ?? string.Empty
        });
    }
    
    public OnError(HttpStatusCode statusCode, IEnumerable<ValidationFailure> errors, string? error)
    {
        StatusCode = statusCode;
        Error = error;
        Errors = errors.Select(err => new ErrorBody
        {
            Property = err.PropertyName,
            Message = err.ErrorMessage,
            Value = err.AttemptedValue ?? string.Empty
        });
    }
    
    public OnError(HttpStatusCode statusCode, string? error)
    {
        StatusCode = statusCode;
        Error = error;
    }
}