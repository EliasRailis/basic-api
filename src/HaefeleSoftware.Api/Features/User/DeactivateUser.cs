using System.Net;
using FluentValidation;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Types;
using MediatR;
using Serilog;

namespace HaefeleSoftware.Api.Features.User;

public sealed class DeactivateUserEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("users/deactivate/{id:int}", async (int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new DeactivateUserCommand(id));
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization(IdentityRoles.Admin);
    }
}

public sealed class DeactivateUserCommand : IRequest<Result<OnSuccess<DeactivateUserResponse>, OnError>>
{
    public int UserId { get; }
    
    public DeactivateUserCommand(int userId)
    {
        UserId = userId;
    }
}

public sealed class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand,
    Result<OnSuccess<DeactivateUserResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly IUserRepository _userRepository;
    private readonly CurrentUser? _currentUser;

    public DeactivateUserCommandHandler(ILogger logger, IUserRepository userRepository, 
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _userRepository = userRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<DeactivateUserResponse>, OnError>> Handle(DeactivateUserCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.User? user = await _userRepository.GetUserByIdAsync(request.UserId);
            
            if (user is null)
            {
                return new OnError(HttpStatusCode.NotFound, "User not found.");
            }

            user.IsActive = false;
            user.LastModifiedBy = _currentUser?.Email;
            bool deactivated = await _userRepository.UpdateUserAsync(user);
            
            return new OnSuccess<DeactivateUserResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new DeactivateUserResponse
                {
                    IsSuccess = deactivated,
                    Message = deactivated ? "User deactivated." : "User not deactivated."
                }
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new OnError(HttpStatusCode.BadRequest, ex.Message);
        }
        finally
        {
            _logger.Information("Request completed.");
        }
    }
}

public sealed class DeactivateUserValidator : AbstractValidator<DeactivateUserCommand>
{
    public DeactivateUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}

public sealed class DeactivateUserResponse
{
    public bool IsSuccess { get; set; }

    public string Message { get; set; } = default!;
}