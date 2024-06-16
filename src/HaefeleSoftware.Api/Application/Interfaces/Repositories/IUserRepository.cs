using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Entities;

namespace HaefeleSoftware.Api.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<CurrentUser?> GetCurrentUserByIdAsync(int id);
    
    Task<User?> GetUserByIdAsync(int id);
    
    Task<User?> GetUserByEmailAsync(string email);
    
    Task<bool> DoesEmailExistAsync(string email);
    
    Task<bool> AddUserAsync(User user);
    
    Task<bool> UpdateUserAsync(User user);
}