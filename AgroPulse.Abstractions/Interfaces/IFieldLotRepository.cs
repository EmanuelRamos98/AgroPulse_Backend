using AgroPulse.Abstractions.DTOs.Lost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Abstractions.Interfaces
{
    public interface IFieldLotRepository
    {
        Task<FieldLotDto> CreateAsync(int userId, SaveFieldLotDto _dto);
        Task<bool> DeleteAsync(int id, int userId);
        Task<List<FieldLotDto>> GetAllByUserIdAsync(int userId);
        Task<List<FieldLotDto>> GetAllLostAsync();
        Task<FieldLotDto?> GetByIdAsync(int id, int userId);
        Task<bool> UpdateAsync(int id, int userId, SaveFieldLotDto _dto);
    }
}
