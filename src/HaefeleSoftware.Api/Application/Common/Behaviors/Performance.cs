using System.Diagnostics;
using MediatR;
using Serilog;

namespace HaefeleSoftware.Api.Application.Common.Behaviors;

public sealed class Performance<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger _logger;

    public Performance(ILogger logger)
    {
        _timer = new Stopwatch();
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        _timer.Start();
        var response = await next();
        _timer.Stop();
        
        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds <= 500) return response;
        
        var requestName = typeof(TRequest).Name;
        _logger.Warning("Long running request {RequestName} ({ElapsedMilliseconds} milliseconds)", 
            requestName, elapsedMilliseconds);
        
        return response;

    }
}