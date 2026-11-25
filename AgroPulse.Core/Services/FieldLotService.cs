using AgroPulse.Abstractions.DTOs.Lost;
using AgroPulse.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Core.Services
{
    public class FieldLotService(IFieldLotRepository _lotRepository) : IFieldLotService
    {
        public Task<List<FieldLotDto>> GetUserLotsAsync(int userId) 
            => _lotRepository.GetAllByUserIdAsync(userId);

        public Task<FieldLotDto?> GetLotByIdAsync(int id, int userId)
            => _lotRepository.GetByIdAsync(id, userId);

        public async Task<FieldLotDto> CreateLotAsync(int userId, SaveFieldLotDto dto)
        {
            ValidateLot(dto);
            return await _lotRepository.CreateAsync(userId, dto);
        }

        public async Task<bool> UpdateLotAsync(int id, int userId, SaveFieldLotDto dto)
        {
            ValidateLot(dto);
            return await _lotRepository.UpdateAsync(id, userId, dto);
        }

        public Task<bool> DeleteLotAsync(int id, int userId)
            => _lotRepository.DeleteAsync(id, userId);

        private static void ValidateLot(SaveFieldLotDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("El nombre del lote es obligatorio.");

            if (dto.Latitude < -90 || dto.Latitude > 90 || dto.Longitude < -180 || dto.Longitude > 180)
                throw new ArgumentException("Las coordenadas GPS ingresadas no son válidas.");

            if (dto.Hectares <= 0)
                throw new ArgumentException("La superficie en hectáreas debe ser mayor a cero.");
        }
    }
}
