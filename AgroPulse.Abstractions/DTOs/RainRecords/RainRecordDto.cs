using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Abstractions.DTOs.RainRecords
{
    public class RainRecordDto
    {
        public int Id { get; set; }
        public int FieldLotId { get; set; }
        public DateOnly RecordDate { get; set; }
        public decimal Millimeters { get; set; }
        public byte Source { get; set; } 
        public string SourceName { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
