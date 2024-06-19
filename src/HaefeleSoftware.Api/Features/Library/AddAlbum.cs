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

public sealed class AddAlbumEndpoint : IEndpoint
{
    private readonly IMapper _mapper;

    public AddAlbumEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("libraries/add", async (AddAlbumRequest request, IMediator mediator) =>
        {
            var command = _mapper.Map<AddAlbumCommand>(request);
            var result = await mediator.Send(command);
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization(IdentityRoles.Customer);
    }
}

public sealed class AddAlbumCommand : IRequest<Result<OnSuccess<AddAlbumResponse>, OnError>>
{
    public int AlbumId { get; init; }

    public int LibraryId { get; init; }
}

public sealed class AddAlbumCommandHandler : IRequestHandler<AddAlbumCommand,
    Result<OnSuccess<AddAlbumResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly ILibraryRepository _libraryRepository;
    private readonly CurrentUser? _currentUser;

    public AddAlbumCommandHandler(ILogger logger, ILibraryRepository libraryRepository, 
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _libraryRepository = libraryRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<AddAlbumResponse>, OnError>> Handle(AddAlbumCommand request, 
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
            
            if (library.LibraryAlbums.Any(x => x.FK_AlbumId == request.AlbumId))
            {
                return new OnError(HttpStatusCode.BadRequest, "Album already exists in library.");
            }
            
            library.LibraryAlbums.Add(new LibraryAlbum
            {
                FK_AlbumId = request.AlbumId,
                CreatedBy = _currentUser?.Email!
            });
            
            bool added = await _libraryRepository.UpdateAlbumsInLibraryAsync(library.LibraryAlbums);
            
            return new OnSuccess<AddAlbumResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new AddAlbumResponse
                {
                    IsAdded = added,
                    Message = added ? "Album added to library." : "Album not added to library."
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

public sealed class AddAlbumValidator : AbstractValidator<AddAlbumCommand>
{
    public AddAlbumValidator()
    {
        RuleFor(x => x.AlbumId)
            .NotEmpty()
            .WithMessage("AlbumId is required.");
        
        RuleFor(x => x.LibraryId)
            .NotEmpty()
            .WithMessage("LibraryId is required.");
    }
}

public sealed class AddAlbumMappingProfile : Profile
{
    public AddAlbumMappingProfile()
    {
        CreateMap<AddAlbumRequest, AddAlbumCommand>();
    }
}

public sealed class AddAlbumRequest
{
    public int AlbumId { get; init; }

    public int LibraryId { get; init; }
}

public sealed class AddAlbumResponse
{
    public bool IsAdded { get; init; }
    
    public string Message { get; init; } = default!;
}