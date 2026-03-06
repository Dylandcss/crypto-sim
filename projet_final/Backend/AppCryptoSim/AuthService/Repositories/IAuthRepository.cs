using AuthService.DTOs;
using AuthService.Models;

namespace AuthService.Repositories;

public interface IAuthRepository
{
    Task<User> AddUserAsync(User user);
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByEmailAsync(string userEmail);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<decimal> GetUserBalanceAsync(int userId);
    Task UpdateUserBalanceAsync(int userId, decimal amount);
}