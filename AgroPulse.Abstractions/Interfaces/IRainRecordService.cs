using AgroPulse.Abstractions.DTOs.RainRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Abstractions.Interfaces
{
    public interface IRainRecordService
    {
        Task<RainRecordDto?> AddManualRainAsync(int lotId, int userId, CreateRainRecordDto dto);
        Task<bool> DeleteRainRecordAsync(int recordId, int lotId, int userId);
        Task<RainSummaryDto?> GetLotRainSummaryAsync(int lotId, int userId);
    }
}
