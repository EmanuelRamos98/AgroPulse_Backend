using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Abstractions.DTOs.Lost
{
    public class SaveFieldLotDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Hectares { get; set; }
        public byte CropType { get; set; }  
        public byte CropStage { get; set; }  
        public DateOnly? SowingDate { get; set; }
    }
}
