using AuthService.Dtos;
using AuthService.Models;

namespace AuthService.Services;

public interface IAuthManagementService
{
        Task<User> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<ProfilResponse> GetCurrentUserAsync(int userId);
        Task<decimal> GetBalanceAsync(int userId);
        Task UpdateBalanceAsync(int userId, decimal amount);
}