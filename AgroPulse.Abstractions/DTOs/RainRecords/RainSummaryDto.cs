using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Abstractions.DTOs.RainRecords
{
    public class RainSummaryDto
    {
        public decimal Last7DaysMm { get; set; }
        public decimal Last30DaysMm { get; set; }
        public decimal CampaignTotalMm { get; set; } 
        public List<RainRecordDto> Records { get; set; } = new();
    }
}
