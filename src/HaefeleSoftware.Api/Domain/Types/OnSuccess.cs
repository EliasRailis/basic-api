using System.Net;

namespace HaefeleSoftware.Api.Domain.Types;

public readonly struct OnSuccess<T> where T : class
{
    public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;

    public DateTime RequestTime => DateTime.UtcNow;

    public T? Response { get; init; }

    public OnSuccess()
    {
    }
}