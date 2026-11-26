using AgroPulse.Abstractions.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Abstractions.Interfaces
{
    public interface IUserRepository
    {
        Task<UserCredentialsDto> CreateAsync(string fullName, string email, string passwordHash);
        Task<bool> EmailExistisAsync(string email);
        Task<UserCredentialsDto?> GetByEmailAsync(string email);
    }
}
