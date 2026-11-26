using AgroPulse.Abstractions.DTOs.Auth;
using AgroPulse.Abstractions.Interfaces;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Core.Services
{
    public class AuthService(IUserRepository _userRepository, IConfiguration _configuration) : IAuthService
    {
        public async Task<AuthResonseDto> RegisterAsync(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new ArgumentException("El nombre completo es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains("@"))
                throw new ArgumentException("El email ingresado no es válido.");

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
                throw new ArgumentException("La contraseña debe tener al menos 6 caracteres.");

            if (await _userRepository.EmailExistisAsync(dto.Email))
                throw new InvalidOperationException("Ya existe una cuenta registrada con ese email.");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var createdUser = await _userRepository.CreateAsync(dto.FullName, dto.Email, passwordHash);

            return GenerateTokenResponse(createdUser);
        }

        public async Task<AuthResonseDto?> LoginAsync(LoginDto dto) 
        {
            if(string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return null;

            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if(user is null) return null;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if(!isPasswordValid) return null;

            return GenerateTokenResponse(user);
        }

        private AuthResonseDto GenerateTokenResponse(UserCredentialsDto user)
        {
            var jwtKey = _configuration["Jwt:key"]
                ?? throw new InvalidOperationException("Falta configurar Jwt:Key en User Secrets.");
            var issuer = _configuration["Jwt:Issuer"] ?? "AgroPulseAPI";
            var audience = _configuration["Jwt:Audience"] ?? "AgroPulseFrontend";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName)
            };

            var expiration = DateTime.UtcNow.AddDays(7);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return new AuthResonseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
