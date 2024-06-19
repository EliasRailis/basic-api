using System.Net;
using FluentValidation;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Types;
using MediatR;
using Serilog;

namespace HaefeleSoftware.Api.Features.Album;

public sealed class DeleteAlbumEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("albums/delete/{id:int}", async (int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new DeleteAlbumCommand(id));
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization(IdentityRoles.Admin);
    }
}

public sealed class DeleteAlbumCommand : IRequest<Result<OnSuccess<DeleteAlbumResponse>, OnError>>
{
    public int AlbumId { get; }
    
    public DeleteAlbumCommand(int albumId)
    {
        AlbumId = albumId;
    }
}

public sealed class DeleteAlbumCommandHandler : IRequestHandler<DeleteAlbumCommand, 
    Result<OnSuccess<DeleteAlbumResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly IAlbumRepository _albumRepository;
    private readonly CurrentUser? _currentUser;

    public DeleteAlbumCommandHandler(ILogger logger, IAlbumRepository albumRepository, 
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _albumRepository = albumRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<DeleteAlbumResponse>, OnError>> Handle(DeleteAlbumCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.Album? album = await _albumRepository
                .GetAlbumByIdAsync(request.AlbumId);
            
            if (album is null)
            {
                return new OnError(HttpStatusCode.NotFound, "Album not found.");
            }
            
            album.IsDeleted = true;
            album.LastModifiedBy = _currentUser?.Email;
            bool deleted = await _albumRepository.UpdateAlbumAsync(album);
            
            return new OnSuccess<DeleteAlbumResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new DeleteAlbumResponse
                {
                    IsDeleted = deleted,
                    Message = deleted ? "Album deleted." : "Album not deleted."
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

public sealed class DeleteAlbumValidator : AbstractValidator<DeleteAlbumCommand>
{
    public DeleteAlbumValidator()
    {
        RuleFor(x => x.AlbumId)
            .NotEmpty()
            .WithMessage("Album ID is required.");
    }
}

public sealed class DeleteAlbumResponse
{
    public bool IsDeleted { get; init; }
    
    public string Message { get; init; } = default!;
}