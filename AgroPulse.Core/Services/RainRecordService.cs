using AgroPulse.Abstractions.DTOs.RainRecords;
using AgroPulse.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Core.Services
{
    public class RainRecordService(IRainRecordRepository _rainRepository, IFieldLotRepository _lotRepository) : IRainRecordService
    {
        public async Task<RainSummaryDto?> GetLotRainSummaryAsync(int lotId, int userId)
        {
            var lot = await _lotRepository.GetByIdAsync(lotId, userId);
            if(lot is null) return null;

            var allRecords = await _rainRepository.GetByLotIdAsync(lotId);
            var today = DateOnly.FromDateTime(DateTime.Today);

            //Por cada fecha, nos quedamos con el Manual (Source=2) si existe; si no, Satelital (Source=1)
            var effectiveRecords = allRecords
                    .GroupBy(r => r.RecordDate)
                    .Select(g => g.OrderByDescending(r => r.Source).First())
                    .OrderByDescending(r => r.RecordDate)
                    .ToList();

            decimal last7Days = effectiveRecords
                .Where(r => r.RecordDate >= today.AddDays(-7))
                .Sum(r => r.Millimeters);

            decimal last30Days = effectiveRecords
                .Where(r => r.RecordDate >= today.AddDays(-30))
                .Sum(r => r.Millimeters);

            // Si el lote tiene fecha de siembra, calculamos el acumulado desde ese día; si no, últimos 90 días
            var campaignStart = lot.SowingDate ?? today.AddDays(-90);
            decimal campaignTotal = effectiveRecords
                .Where(r => r.RecordDate >= campaignStart)
                .Sum(r => r.Millimeters);

            return new RainSummaryDto
            {
                Last7DaysMm = last7Days,
                Last30DaysMm = last30Days,
                CampaignTotalMm = campaignTotal,
                Records = effectiveRecords
            };
        }

        public async Task<RainRecordDto?> AddManualRainAsync(int lotId, int userId, CreateRainRecordDto dto)
        {
            if(dto.Millimeters <= 0 || dto.Millimeters > 1000)
                throw new ArgumentException("Los milímetros registrados deben estar entre 0.1 y 1000 mm.");

            var lot = await _lotRepository.GetByIdAsync(lotId, userId);
            if (lot is null) return null;

            return await _rainRepository.AddManualRecordAsync(lotId, dto);
        }

        public async Task<bool> DeleteRainRecordAsync(int recordId, int lotId, int userId)
        {
            var lot = await _lotRepository.GetByIdAsync(lotId, userId);
            if(lot is null) return false;

            return await _rainRepository.DeleteRecordAsync(recordId, lotId);
        }
    }
}
