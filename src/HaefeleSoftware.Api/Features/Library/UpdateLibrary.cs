using System.Net;
using AutoMapper;
using FluentValidation;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Entities;
using HaefeleSoftware.Api.Domain.Types;
using MediatR;
using Serilog;

namespace HaefeleSoftware.Api.Features.Library;

public sealed class UpdateLibraryEndpoint : IEndpoint
{
    private readonly IMapper _mapper;

    public UpdateLibraryEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("library/update", async (UpdateLibraryRequest request, ISender sender) =>
        {
            var command = _mapper.Map<UpdateLibraryCommand>(request);
            var result = await sender.Send(command);
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization(IdentityRoles.Customer);
    }
}

public sealed class UpdateLibraryCommand : IRequest<Result<OnSuccess<UpdateLibraryResponse>, OnError>>
{
    public int LibraryId { get; init; }
    
    public string Name { get; init; } = default!;
}

public sealed class UpdateLibraryCommandHandler : IRequestHandler<UpdateLibraryCommand,
    Result<OnSuccess<UpdateLibraryResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly ILibraryRepository _libraryRepository;
    private readonly CurrentUser? _currentUser;

    public UpdateLibraryCommandHandler(ILogger logger, ILibraryRepository libraryRepository, 
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _libraryRepository = libraryRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<UpdateLibraryResponse>, OnError>> Handle(UpdateLibraryCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.Library? library = await _libraryRepository
                .GetLibraryByIdAsync(request.LibraryId);
            
            if (library is null)
            {
                return new OnError(HttpStatusCode.NotFound, "Library not found.");
            }

            Domain.Entities.User? userLibraries = await _libraryRepository
                .GetUserLibrariesByIdAsync(library.FK_UserId);
            
            if (userLibraries is null)
            {
                return new OnError(HttpStatusCode.NotFound, "User not found.");
            }

            if (userLibraries.Libraries.Where(x => !x.IsDeleted).Any(x => x.Name == request.Name.Trim()))
            {
                return new OnError(HttpStatusCode.BadRequest, "Library name already exists.");
            }
            
            library.Name = request.Name;
            library.LastModifiedBy = _currentUser?.Email;
            bool updated = await _libraryRepository.UpdateLibraryAsync(library);
            
            return new OnSuccess<UpdateLibraryResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new UpdateLibraryResponse
                {
                    IsUpdated = updated,
                    Message = updated ? "Library updated." : "Library not updated."
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

public sealed class UpdateLibraryValidator : AbstractValidator<UpdateLibraryCommand>
{
    public UpdateLibraryValidator()
    {
        RuleFor(x => x.LibraryId)
            .NotEmpty()
            .WithMessage("LibraryId is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
    }
}

public sealed class UpdateLibraryMappingProfile : Profile
{
    public UpdateLibraryMappingProfile()
    {
        CreateMap<UpdateLibraryRequest, UpdateLibraryCommand>();
    }
}

public sealed class UpdateLibraryRequest
{
    public int LibraryId { get; init; }
    
    public string Name { get; init; } = default!;
}

public sealed class UpdateLibraryResponse
{
    public bool IsUpdated { get; set; }
    
    public string Message { get; set; } = default!;
}