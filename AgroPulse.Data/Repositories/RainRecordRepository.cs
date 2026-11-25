using AgroPulse.Abstractions.DTOs.RainRecords;
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
    public class RainRecordRepository(AgroPulseDBContext _context) : IRainRecordRepository
    {
        public async Task<List<RainRecordDto>> GetByLotIdAsync(int fieldLotId)
        {
            var records = await _context.RainRecords
                .AsNoTracking() 
                .Where(r => r.FieldLotId == fieldLotId)
                .OrderByDescending(r => r.RecordDate)
                .ToListAsync();

            return records.Select(MapToDto).ToList();
        }

        public async Task<RainRecordDto> AddManualRecordAsync(int fieldLotId, CreateRainRecordDto _dto)
        {
            var entity = new RainRecord
            {
                FieldLotId = fieldLotId,
                RecordDate = _dto.RecordDate,
                Millimeters = _dto.Millimeters,
                Source = 2,
                Notes = _dto.Notes?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.RainRecords.Add(entity);
            await _context.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task<bool> DeleteRecordAsync(int recordId, int fieldLotId)
        {
            var entity = await _context.RainRecords
                .FirstOrDefaultAsync(r => r.Id == recordId && r.FieldLotId ==fieldLotId);

            if(entity == null) return false;

            _context.RainRecords.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task SyncSatellitalRecordAsync(int fieldLotId, DateOnly date, decimal millimeters)
        {
            var existing = await _context.RainRecords
                .FirstOrDefaultAsync(r => r.FieldLotId == fieldLotId && r.RecordDate == date && r.Source == 1);

            if (existing is not null)
            {
                existing.Millimeters = millimeters;
            }
            else
            {
                _context.RainRecords.Add(new RainRecord
                {
                    FieldLotId = fieldLotId,
                    RecordDate = date,
                    Millimeters = millimeters,
                    Source = 1,
                    Notes = "Registro automático satelital (Open-Meteo)",
                    CreatedAt = DateTime.UtcNow
                });
            }
            await _context.SaveChangesAsync();
        }

        //Mapeo
        private static RainRecordDto MapToDto(RainRecord entity) => new()
        {
            Id = entity.Id,
            FieldLotId = entity.FieldLotId,
            RecordDate = entity.RecordDate,
            Millimeters = entity.Millimeters,
            Source = (byte)entity.Source,
            SourceName = entity.Source == 2 ? "Pluviometro (Manual)" : "Satelital (API)",
            Notes = entity.Notes,
        };
    }
}
