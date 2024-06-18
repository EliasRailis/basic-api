using System.Net;
using AutoMapper;
using FluentValidation;
using HaefeleSoftware.Api.Application.Common.Extensions;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Enums;
using HaefeleSoftware.Api.Domain.Types;
using MediatR;
using Serilog;

namespace HaefeleSoftware.Api.Features.Authentication;

public sealed class RegisterEndpoint : IEndpoint
{
    private readonly IMapper _mapper;

    public RegisterEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("register", async (RegisterRequest request, ISender sender) =>
        {
            var command = _mapper.Map<RegisterCommand>(request);
            var result = await sender.Send(command);
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1);
    }
}

public sealed class RegisterCommand : IRequest<Result<OnSuccess<RegisterResponse>, OnError>>
{
    public string FirstName { get; init; } = default!;

    public string LastName { get; init; } = default!;

    public string Email { get; init; } = default!;

    public string Password { get; init; } = default!;
}

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand,
    Result<OnSuccess<RegisterResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly IUserRepository _userRepository;

    public RegisterCommandHandler(ILogger logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<Result<OnSuccess<RegisterResponse>, OnError>> Handle(RegisterCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            bool emailExists = await _userRepository.DoesEmailExistAsync(request.Email);

            if (emailExists)
            {
                return new OnError(HttpStatusCode.BadRequest, "This account already exists.");
            }

            var user = new Domain.Entities.User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim(),
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FK_RoleId = (int)Roles.Customer,
                CreatedBy = "api",
                IsActive = true,
                IsDeleted = false
            };

            bool added = await _userRepository.AddUserAsync(user);

            return new OnSuccess<RegisterResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new RegisterResponse
                {
                    IsSuccess = added,
                    Message = added ? "User registered successfully" : "Failed to register user"
                }
            };
        }
        catch (Exception ex)
        {
            _logger.Error("Error while user registering: {ErrorMessage}", ex.Message);
            return new OnError(HttpStatusCode.BadRequest, "Error while user registering.");
        }
        finally
        {
            _logger.Information("User {UserEmail} registration", request.Email);
        }
    }
}

public sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is invalid");

        RuleFor(x => x.Password).Password();

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required");
    }
}

public sealed class RegisterMappingProfile : Profile
{
    public RegisterMappingProfile()
    {
        CreateMap<RegisterRequest, RegisterCommand>();
    }
}

public sealed class RegisterRequest
{
    public string FirstName { get; init; } = default!;

    public string LastName { get; init; } = default!;

    public string Email { get; init; } = default!;

    public string Password { get; init; } = default!;
}

public sealed class RegisterResponse
{
    public bool IsSuccess { get; set; }

    public required string Message { get; set; } = default!;
}