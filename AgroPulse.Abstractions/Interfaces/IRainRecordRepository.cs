using AgroPulse.Abstractions.DTOs.RainRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Abstractions.Interfaces
{
    public interface IRainRecordRepository
    {
        Task<RainRecordDto> AddManualRecordAsync(int fieldLotId, CreateRainRecordDto _dto);
        Task<bool> DeleteRecordAsync(int recordId, int fieldLotId);
        Task<List<RainRecordDto>> GetByLotIdAsync(int fieldLotId);
        Task SyncSatellitalRecordAsync(int fieldLotId, DateOnly date, decimal millimeters);
    }
}
