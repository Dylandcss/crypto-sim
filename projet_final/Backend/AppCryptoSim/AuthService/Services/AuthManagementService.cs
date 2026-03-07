using AuthService.Dtos;
using AuthService.Exceptions;
using AuthService.Extensions;
using AuthService.Models;
using AuthService.Repositories;
using CryptoSim.Shared.Enums;
using CryptoSim.Shared.Exceptions;


namespace AuthService.Services;


public class AuthManagementService : IAuthManagementService
{
    private readonly IAuthRepository _authRepository;
    private readonly JwtTokenService _jwtTokenService;
    
    public AuthManagementService(IAuthRepository authRepository, JwtTokenService jwtService)
    {
        _authRepository = authRepository;
        _jwtTokenService = jwtService;
    }
    
    public async Task<User> RegisterAsync(RegisterRequest request)
    {        
        if (await _authRepository.GetUserByEmailAsync(request.Email) != null)
            throw new UserAlreadyExistsException(request.Email);

        if (await _authRepository.GetUserByUsernameAsync(request.Username) != null)
            throw new UserAlreadyExistsException(request.Username);

        var newUser = new User()
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = PasswordService.HashPassword(request.Password),
            Role = Role.User, 
            Balance = 10_000m 
        };

        var user = await _authRepository.AddUserAsync(newUser);
        return user;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var existingUser = await _authRepository.GetUserByUsernameAsync(request.Username);

        if (existingUser is null || !PasswordService.VerifyPassword(request.Password, existingUser.PasswordHash))
            throw new UnauthorizedException("Nom d'utilisateur ou mot de passe invalide");

        var (token, expiration) = _jwtTokenService.GenerateToken(existingUser);

        return new AuthResponse (
            token,
            existingUser.Username,
            existingUser.Role.ToString(),
            existingUser.Balance,
            expiration * 60
        );
    }

    public async Task<ProfilResponse> GetCurrentUserAsync(int userId)
    {
        var user = await _authRepository.GetUserByIdAsync(userId);
        return user is null ? throw new UserNotFoundException(userId) : user.ToProfilResponse();
    }

    public async Task<decimal> GetBalanceAsync(int userId)
    {
        return await _authRepository.GetUserBalanceAsync(userId);
    }

    public async Task UpdateBalanceAsync(int userId, decimal amount)
    {
        await _authRepository.UpdateUserBalanceAsync(userId, amount);
    }
}