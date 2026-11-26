using AgroPulse.Abstractions.DTOs.Auth;
using AgroPulse.Abstractions.Interfaces;
using AgroPulse.Data.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Data.Repositories
{
    public class UserRepository(AgroPulseDBContext _context) : IUserRepository
    {
        public async Task<UserCredentialsDto?> GetByEmailAsync(string email)
        {
            var normalizedEmail = email.Trim().ToLower();
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == normalizedEmail);

            if (user is null) return null;

            return new UserCredentialsDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
            };
        }

        public async Task<bool> EmailExistisAsync(string email) 
        {
            var normalizedEmail = email.Trim().ToLower();
            return await _context.Users.AnyAsync(u => u.Email == normalizedEmail);
        }

        public async Task<UserCredentialsDto> CreateAsync(string fullName, string email, string passwordHash)
        {
            var entity = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(entity);
            await _context.SaveChangesAsync();

            return new UserCredentialsDto
            {
                Id = entity.Id,
                FullName = entity.FullName,
                Email = entity.Email,
                PasswordHash = entity.PasswordHash,
            };
        }
    }
}
