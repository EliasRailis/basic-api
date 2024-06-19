using System.Net;
using FluentValidation;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Dto;
using HaefeleSoftware.Api.Domain.Types;
using MediatR;
using Serilog;

namespace HaefeleSoftware.Api.Features.Library;

public sealed class GetLibrariesEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("libraries", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetLibrariesQuery());
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization(IdentityRoles.Customer);
    }
}

public sealed class GetLibrariesQuery : IRequest<Result<OnSuccess<GetLibrariesResponse>, OnError>>;

public sealed class GetLibrariesQueryHandler : IRequestHandler<GetLibrariesQuery,
    Result<OnSuccess<GetLibrariesResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly ILibraryRepository _libraryRepository;
    private readonly CurrentUser? _currentUser;

    public GetLibrariesQueryHandler(ILogger logger, ILibraryRepository libraryRepository, 
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _libraryRepository = libraryRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<GetLibrariesResponse>, OnError>> Handle(GetLibrariesQuery request, 
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

            var response = new List<LibraryInformationDto>();
            foreach (var library in userLibraries.Libraries.Where(x => !x.IsDeleted))
            {
                response.Add(new LibraryInformationDto
                {
                    Id = library.Id,
                    LibraryName = library.Name,
                    CreatedAt = library.Created.ToString("yyyy-M-d dddd"),
                    AlbumsCount = library.LibraryAlbums.Count,
                    Albums = library.LibraryAlbums.Select(x => new SmallAlbumDto
                    {
                        AlbumId = x.Album.Id,
                        AlbumName = x.Album.Name
                    })
                });
            }
            
            return new OnSuccess<GetLibrariesResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new GetLibrariesResponse(response)
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

public sealed class GetLibrariesValidator : AbstractValidator<GetLibrariesQuery>
{
    public GetLibrariesValidator() { }
}

public sealed class GetLibrariesResponse
{
    public IEnumerable<LibraryInformationDto> Libraries { get; } 
    
    public GetLibrariesResponse(IEnumerable<LibraryInformationDto> libraries)
    {
        Libraries = libraries;
    }
}