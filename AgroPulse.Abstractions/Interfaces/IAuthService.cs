using AgroPulse.Abstractions.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Abstractions.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResonseDto?> LoginAsync(LoginDto dto);
        Task<AuthResonseDto> RegisterAsync(RegisterDto dto);
    }
}
