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

public sealed class CreateArtistEndpoint : IEndpoint
{
    private readonly IMapper _mapper;

    public CreateArtistEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("artist/create", async (CreateArtistRequest request, ISender sender) =>
        {
            var command = _mapper.Map<CreateArtistCommand>(request);
            var result = await sender.Send(command);
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization(IdentityRoles.Admin);
    }
}

public sealed class CreateArtistCommand : IRequest<Result<OnSuccess<CreateArtistResponse>, OnError>>
{
    public string Name { get; init; } = default!;
}

public sealed class CreateArtistCommandHandler : IRequestHandler<CreateArtistCommand,
    Result<OnSuccess<CreateArtistResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly IArtistRepository _artistRepository;
    private readonly CurrentUser? _currentUser;

    public CreateArtistCommandHandler(ILogger logger, IArtistRepository artistRepository, 
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _artistRepository = artistRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<CreateArtistResponse>, OnError>> Handle(CreateArtistCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            bool doesArtistExist = await _artistRepository.DoesArtistExistAsync(request.Name);
            
            if (doesArtistExist)
            {
                return new OnError(HttpStatusCode.BadRequest, "Artist already exists.");
            }

            var artist = new Domain.Entities.Artist
            {
                Name = request.Name.Trim(),
                CreatedBy = _currentUser?.Email!,
                IsDeleted = false
            };

            bool created = await _artistRepository.AddArtistAsync(artist);
            
            return new OnSuccess<CreateArtistResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new CreateArtistResponse
                {
                    IsSuccess = created,
                    ArtistId = artist.Id,
                    Message = created ? "Artist created." : "Artist not created."
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

public sealed class CreateArtistValidator : AbstractValidator<CreateArtistCommand>
{
    public CreateArtistValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Artist name is required.");
    }
}

public sealed class CreateArtistMappingProfile : Profile
{
    public CreateArtistMappingProfile()
    {
        CreateMap<CreateArtistRequest, CreateArtistCommand>();
    }
}

public sealed class CreateArtistRequest
{
    public string Name { get; init; } = default!;
}

public sealed class CreateArtistResponse
{
    public bool IsSuccess { get; set; }
    
    public int ArtistId { get; set; }

    public string Message { get; set; } = default!;
}