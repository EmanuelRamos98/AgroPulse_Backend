using AgroPulse.Abstractions.DTOs.Lost;
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
    public class FieldLotRepository(AgroPulseDBContext _context) : IFieldLotRepository
    {
        public async Task<List<FieldLotDto>> GetAllByUserIdAsync(int userId)
        {
            var lost = await _context.FieldLots
                    .AsNoTracking()
                    .Where(l => l.UserId == userId)
                    .OrderByDescending(l => l.CreatedAt)
                    .ToListAsync();
            return lost.Select(MapToDto).ToList();
        }

        public async Task<FieldLotDto?> GetByIdAsync(int id, int userId)
        {
            var lot = await _context.FieldLots
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
            return lot is null ? null : MapToDto(lot);
        }

        public async Task<FieldLotDto> CreateAsync(int userId, SaveFieldLotDto _dto)
        {
            var entity = new FieldLot
            {
                UserId = userId,
                Name = _dto.Name.Trim(),
                Latitude = _dto.Latitude,
                Longitude = _dto.Longitude,
                Hectares = _dto.Hectares,
                CropType = (sbyte)_dto.CropType,
                CropStage = (sbyte)_dto.CropStage,
                SowingDate = _dto.SowingDate,
                CreatedAt = DateTime.UtcNow,
            };

            _context.FieldLots.Add(entity);
            await _context.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task<bool> UpdateAsync(int id, int userId, SaveFieldLotDto _dto)
        {
            var entity = await _context.FieldLots
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

            if (entity is null) return false;

            entity.Name = _dto.Name.Trim();
            entity.Latitude = _dto.Latitude;
            entity.Longitude = _dto.Longitude;
            entity.Hectares = _dto.Hectares;
            entity.CropType = (sbyte)_dto.CropType;
            entity.CropStage = (sbyte)_dto.CropStage;
            entity.SowingDate = _dto.SowingDate;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var entity = await _context.FieldLots
                    .FirstOrDefaultAsync(l => l.Id==id && l.UserId == userId);

            if (entity == null) return false;

            _context.FieldLots.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<FieldLotDto>> GetAllLostAsync()
        {
            var lots = await _context.FieldLots.AsNoTracking().ToListAsync();
            return lots.Select(MapToDto).ToList();
        }

        //Mapeo 
        private static FieldLotDto MapToDto(FieldLot _entity) => new()
        {
            Id = _entity.Id,
            Name = _entity.Name,
            Latitude = _entity.Latitude,
            Longitude = _entity.Longitude,
            Hectares = _entity.Hectares,
            CropType = (byte)_entity.CropType,
            CropTypeName = GetCropTypeName((byte)_entity.CropType),
            CropStage = (byte)_entity.CropStage,
            CropStageName = GetCropStageName((byte)_entity.CropStage),
            SowingDate = _entity.SowingDate,
        };

        private static string GetCropTypeName(byte type) => type switch
        {
            0 => "Barbecho",
            1 => "Soja",
            2 => "Maíz",
            3 => "Trigo",
            4 => "Girasol",
            5 => "Pastura",
            _ => "Otro"
        };

        private static string GetCropStageName(byte stage) => stage switch
        {
            0 => "Pre-Siembra",
            1 => "Siembra",
            2 => "Vegetativo",
            3 => "Floración Crítica",
            4 => "Llenado de Grano",
            5 => "Cosecha",
            _ => "Sin definir"
        };
    }
}
