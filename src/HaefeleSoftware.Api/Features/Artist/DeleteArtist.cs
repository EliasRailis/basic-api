using System.Net;
using FluentValidation;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Types;
using MediatR;
using Serilog;

namespace HaefeleSoftware.Api.Features.Artist;

public sealed class DeleteArtistEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("artists/delete/{id:int}", async (int id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteArtistCommand(id));
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization(IdentityRoles.Admin);
    }
}

public sealed class DeleteArtistCommand : IRequest<Result<OnSuccess<DeleteArtistResponse>, OnError>>
{
    public int ArtistId { get; }

    public DeleteArtistCommand(int artistId)
    {
        ArtistId = artistId;
    }
}

public sealed class DeleteArtistCommandHandler : IRequestHandler<DeleteArtistCommand,
    Result<OnSuccess<DeleteArtistResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly IArtistRepository _artistRepository;
    private readonly CurrentUser? _currentUser;

    public DeleteArtistCommandHandler(ILogger logger, IArtistRepository artistRepository, 
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _artistRepository = artistRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<DeleteArtistResponse>, OnError>> Handle(DeleteArtistCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.Artist? artist = await _artistRepository
                .GetArtistByIdAsync(request.ArtistId);

            if (artist is null)
            {
                return new OnError(HttpStatusCode.NotFound, "Artist not found.");
            }

            artist.IsDeleted = true;
            artist.LastModifiedBy = _currentUser?.Email;
            bool deleted = await _artistRepository.UpdateArtistAsync(artist);
            
            return new OnSuccess<DeleteArtistResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new DeleteArtistResponse
                {
                    IsDeleted = deleted,
                    Message = deleted ? "Artist deleted." : "Artist not deleted."
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

public sealed class DeleteArtistValidator : AbstractValidator<DeleteArtistCommand>
{
    public DeleteArtistValidator()
    {
        RuleFor(x => x.ArtistId)
            .NotEmpty()
            .WithMessage("Artist ID is required.");
    }
}

public sealed class DeleteArtistResponse
{
    public bool IsDeleted { get; init; }
    
    public string Message { get; init; } = default!;
}