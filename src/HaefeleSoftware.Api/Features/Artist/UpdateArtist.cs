using System.Net;
using AutoMapper;
using FluentValidation;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Types;
using MediatR;
using Serilog;

namespace HaefeleSoftware.Api.Features.Artist;

public sealed class UpdateArtistEndpoint : IEndpoint
{
    private readonly IMapper _mapper;

    public UpdateArtistEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("artists/update", async (UpdateArtistRequest request, ISender sender) =>
        {
            var command = _mapper.Map<UpdateArtistCommand>(request);
            var result = await sender.Send(command);
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization(IdentityRoles.Admin);
    }
}

public sealed class UpdateArtistCommand : IRequest<Result<OnSuccess<UpdateArtistResponse>, OnError>>
{
    public int ArtistId { get; init; }
    
    public string Name { get; init; } = default!;
}

public sealed class UpdateArtistCommandHandler : IRequestHandler<UpdateArtistCommand,
    Result<OnSuccess<UpdateArtistResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly IArtistRepository _artistRepository;
    private readonly CurrentUser? _currentUser;

    public UpdateArtistCommandHandler(ILogger logger, IArtistRepository artistRepository, 
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _artistRepository = artistRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<UpdateArtistResponse>, OnError>> Handle(UpdateArtistCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            bool doesArtistExist = await _artistRepository.DoesArtistExistAsync(request.Name);
            
            if (doesArtistExist)
            {
                return new OnError(HttpStatusCode.BadRequest, "Name already exists.");
            }
            
            Domain.Entities.Artist? artist = await _artistRepository.GetArtistByIdAsync(request.ArtistId);
            
            if (artist is null)
            {
                return new OnError(HttpStatusCode.NotFound, "Artist not found.");
            }
            
            artist.Name = request.Name.Trim();
            artist.LastModifiedBy = _currentUser?.Email;
            bool updated = await _artistRepository.UpdateArtistAsync(artist);
            
            return new OnSuccess<UpdateArtistResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new UpdateArtistResponse
                {
                    IsUpdated = updated,
                    Message = updated ? "Artist updated." : "Artist not updated."
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

public sealed class UpdateArtistValidator : AbstractValidator<UpdateArtistCommand>
{
    public UpdateArtistValidator()
    {
        RuleFor(x => x.ArtistId)
            .NotEmpty()
            .WithMessage("Artist ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Artist name is required.");
    }
}

public sealed class UpdateArtistMappingProfile : Profile
{
    public UpdateArtistMappingProfile()
    {
        CreateMap<UpdateArtistRequest, UpdateArtistCommand>();
    }
}

public sealed class UpdateArtistRequest
{
    public int ArtistId { get; init; }
    
    public string Name { get; init; } = default!;
}

public sealed class UpdateArtistResponse
{
    public bool IsUpdated { get; init; }
    
    public string Message { get; init; } = default!;
}