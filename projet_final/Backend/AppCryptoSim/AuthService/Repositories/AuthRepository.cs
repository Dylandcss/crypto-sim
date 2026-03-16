using AuthService.Data;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;


namespace AuthService.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AuthDbContext _context;
    
    public AuthRepository(AuthDbContext context)
    {
        _context = context;
    }
    
    public async Task<User> AddUserAsync(User user)
    {
        var entry = await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<User?> GetUserByEmailAsync(string userEmail)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Email == userEmail);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
    }

    public async Task<decimal> GetUserBalanceAsync(int userId)
    {
        return await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.Balance)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateUserBalanceAsync(int userId, decimal amount)
    {
        var existingUser = await _context.Users.FindAsync(userId);        
        if (existingUser == null) return false;
        
        existingUser.Balance += amount;
        await _context.SaveChangesAsync();
        return true;
    }
}