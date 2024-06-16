using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Infrastructure.Repositories;

namespace HaefeleSoftware.Api.Application.Configurations;

public static class Repositories
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<ITokenRepository, TokenRepository>();
        services.AddTransient<IAlbumRepository, AlbumRepository>();
        services.AddTransient<ISongRepository, SongRepository>();
        services.AddTransient<IArtistRepository, ArtistRepository>();
        services.AddTransient<ILibraryRepository, LibraryRepository>();
    }
}