using AgroPulse.Abstractions.DTOs.Lost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Abstractions.Interfaces
{
    public interface IFieldLotService
    {
        Task<FieldLotDto> CreateLotAsync(int userId, SaveFieldLotDto dto);
        Task<bool> DeleteLotAsync(int id, int userId);
        Task<FieldLotDto?> GetLotByIdAsync(int id, int userId);
        Task<List<FieldLotDto>> GetUserLotsAsync(int userId);
        Task<bool> UpdateLotAsync(int id, int userId, SaveFieldLotDto dto);
    }
}
