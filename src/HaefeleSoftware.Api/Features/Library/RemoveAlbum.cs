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

public sealed class RemoveAlbumEndpoint : IEndpoint
{
    private readonly IMapper _mapper;

    public RemoveAlbumEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("libraries/remove", async (RemoveAlbumRequest request, ISender sender) =>
        {
            var command = _mapper.Map<RemoveAlbumCommand>(request);
            var result = await sender.Send(command);
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization(IdentityRoles.Customer);
    }
}

public sealed class RemoveAlbumCommand : IRequest<Result<OnSuccess<RemoveAlbumResponse>, OnError>>
{
    public int AlbumId { get; init; }

    public int LibraryId { get; init; }
}

public sealed class RemoveAlbumCommandHandler : IRequestHandler<RemoveAlbumCommand,
    Result<OnSuccess<RemoveAlbumResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly ILibraryRepository _libraryRepository;
    private readonly CurrentUser? _currentUser;

    public RemoveAlbumCommandHandler(ILogger logger, ILibraryRepository libraryRepository, 
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _libraryRepository = libraryRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<RemoveAlbumResponse>, OnError>> Handle(RemoveAlbumCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.User? userLibraries = await _libraryRepository
                .GetUserLibrariesByIdAsync(_currentUser!.Id);

            if (userLibraries is null)
            {
                return new OnError(HttpStatusCode.NotFound, "User not found.");
            }
            
            Domain.Entities.Library? library = userLibraries.Libraries
                .FirstOrDefault(x => x.Id == request.LibraryId);
            
            if (library is null)
            {
                return new OnError(HttpStatusCode.NotFound, "Library not found.");
            }

            bool removed = false;
            foreach (var libraryAlbum in library.LibraryAlbums)
            {
                if (libraryAlbum.FK_AlbumId != request.AlbumId) continue;
                
                removed = await _libraryRepository.RemoveAlbumFromLibraryAsync(libraryAlbum);
                break;
            }
            
            return new OnSuccess<RemoveAlbumResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new RemoveAlbumResponse
                {
                    IsRemoved = removed,
                    Message = removed ? "Album removed." : "Album not found."
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

public sealed class RemoveAlbumValidator : AbstractValidator<RemoveAlbumCommand>
{
    public RemoveAlbumValidator()
    {
        RuleFor(x => x.AlbumId)
            .NotEmpty()
            .WithMessage("AlbumId is required.");
        
        RuleFor(x => x.LibraryId)
            .NotEmpty()
            .WithMessage("LibraryId is required.");
    }
}

public sealed class RemoveAlbumMappingProfile : Profile
{
    public RemoveAlbumMappingProfile()
    {
        CreateMap<RemoveAlbumRequest, RemoveAlbumCommand>();
    }
}

public sealed class RemoveAlbumRequest
{
    public int AlbumId { get; init; }

    public int LibraryId { get; init; }
}

public sealed class RemoveAlbumResponse
{
    public bool IsRemoved { get; init; }
    
    public string Message { get; init; } = default!;
}