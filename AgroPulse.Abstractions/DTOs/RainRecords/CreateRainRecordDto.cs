using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Abstractions.DTOs.RainRecords
{
    public class CreateRainRecordDto
    {
        public DateOnly RecordDate { get; set; }
        public decimal Millimeters { get; set; }
        public string? Notes { get; set; }
    }
}
