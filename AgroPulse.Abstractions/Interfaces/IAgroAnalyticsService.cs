using AgroPulse.Abstractions.DTOs.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Abstractions.Interfaces
{
    public interface IAgroAnalyticsService
    {
        Task<LotDashboardDto?> GetLotDashboardDtoAsync(int lotId, int userId);
    }
}
