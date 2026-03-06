using System.Security.Claims;
using AuthService.DTOs;
using AuthService.Exceptions;
using AuthService.Extensions;
using AuthService.Models;
using AuthService.Repositories;
namespace AuthService.Services;

public class AuthManagementService : IAuthManagementService
{
    private readonly IAuthRepository _authRepository;
    private readonly JwtService _jwtService;
    
    public AuthManagementService(IAuthRepository authRepository, JwtService jwtService)
    {
        _authRepository = authRepository;
        _jwtService = jwtService;
    }
    
    public async Task<User> RegisterAsync(RegisterRequest request)
    {
        // Validation des champs requis
        if(string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required");
        if(string.IsNullOrWhiteSpace(request.Username))
            throw new ArgumentException("Username is required");
        if(string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Password is required");
        
        // Vérifier si l'utilisateur existe déjà
        var existingUser = await _authRepository.GetUserByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new UserAlreadyExistsException("This user already exists.");
        }

        // Créer un nouvel utilisateur
        var newUser = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = PasswordService.HashPassword(request.Password), // Hash du mot de passe
        };

        // Ajouter l'utilisateur à la base de données
        var user = await _authRepository.AddUserAsync(newUser);
        return user;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _authRepository.GetUserByUsernameAsync(request.Username);

        if (user == null || !PasswordService.VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid username or password");

        var (token, expiration) = _jwtService.GenerateToken(user);

        return new AuthResponse(
            token,
            user.Username,
            user.Role.ToString(),
            user.Balance,
            expiration
        );
    }

    public async Task<ProfilResponse> GetCurrentUserAsync(int userId)
    {
        var user = await _authRepository.GetUserByIdAsync(userId);
        if (user == null) throw new UserNotFoundException("User not found.");
        var profil = user.ToProfilResponse();
        return profil;
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