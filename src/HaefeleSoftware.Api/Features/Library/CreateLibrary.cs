using System.Net;
using AutoMapper;
using FluentValidation;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Types;
using MediatR;
using Serilog;

namespace HaefeleSoftware.Api.Features.Library;

public sealed class CreateLibraryEndpoint : IEndpoint
{
    private readonly IMapper _mapper;

    public CreateLibraryEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("library/create", async (CreateLibraryRequest request, ISender sender) =>
        {
            var command = _mapper.Map<CreateLibraryCommand>(request);
            var result = await sender.Send(command);
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization(IdentityRoles.Customer);
    }
}

public sealed class CreateLibraryCommand : IRequest<Result<OnSuccess<CreateLibraryResponse>, OnError>>
{
    public string Name { get; init; } = default!;
}

public sealed class CreateLibraryCommandHandler : IRequestHandler<CreateLibraryCommand,
    Result<OnSuccess<CreateLibraryResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly ILibraryRepository _libraryRepository;
    private readonly CurrentUser? _currentUser;

    public CreateLibraryCommandHandler(ILogger logger, ILibraryRepository libraryRepository, 
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _libraryRepository = libraryRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<CreateLibraryResponse>, OnError>> Handle(CreateLibraryCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            bool libraryExists = await _libraryRepository.DoesLibraryExistAsync(request.Name);
            
            if (libraryExists)
            {
                return new OnError(HttpStatusCode.BadRequest, "Library already exists.");
            }

            if (_currentUser!.Email is null || _currentUser!.Id == 0)
            {
                return new OnError(HttpStatusCode.NotFound, "User not found.");
            }
            
            var library = new Domain.Entities.Library
            {
                Name = request.Name,
                CreatedBy = _currentUser!.Email!,
                FK_UserId = _currentUser!.Id
            };
            
            bool create = await _libraryRepository.AddLibraryAsync(library);
            
            return new OnSuccess<CreateLibraryResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new CreateLibraryResponse
                {
                    IsSuccess = create,
                    LibraryId = library.Id,
                    Message = create ? "Library created." : "Library not created."
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

public sealed class CreateLibraryValidator : AbstractValidator<CreateLibraryCommand>
{
    public CreateLibraryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Library name is required.");
    }
}

public sealed class CreateLibraryMappingProfile : Profile
{
    public CreateLibraryMappingProfile()
    {
        CreateMap<CreateLibraryRequest, CreateLibraryCommand>();
    }
}

public sealed class CreateLibraryRequest
{
    public string Name { get; init; } = default!;
}

public sealed class CreateLibraryResponse
{
    public bool IsSuccess { get; init; }
    
    public int LibraryId { get; init; }
    
    public string Message { get; init; } = default!;
}