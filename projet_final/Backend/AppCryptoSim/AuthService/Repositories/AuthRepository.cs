using AuthService.Data;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AuthDbContext _db;
    
    public AuthRepository(AuthDbContext db)
    {
        _db = db;
    }
    
    public async Task<User> AddUserAsync(User user)
    {
        var entry = await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetUserByEmailAsync(string userEmail)
    {
        return await _db.Users.FirstOrDefaultAsync(user => user.Email == userEmail);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(user => user.Username == username);
    }

    public async Task<decimal> GetUserBalanceAsync(int userId)
    {
        var dbUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        return dbUser?.Balance ?? 0;
    }

    public async Task UpdateUserBalanceAsync(int userId, decimal amount)
    {
        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        existingUser?.Balance += amount;
        await _db.SaveChangesAsync();
    }
}