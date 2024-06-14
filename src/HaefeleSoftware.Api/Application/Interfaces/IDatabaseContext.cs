namespace HaefeleSoftware.Api.Application.Interfaces;

public interface IDatabaseContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}