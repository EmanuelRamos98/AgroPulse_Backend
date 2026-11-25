using AgroPulse.Abstractions.DTOs.Lost;
using AgroPulse.Abstractions.DTOs.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Abstractions.DTOs.Analytics
{
    public class SprayingHourDto
    {
        public DateTime Time { get; set; }
        public double Temperature { get; set; }
        public int RelativeHumidity { get; set; }
        public double WindSpeed { get; set; }
        public double WindGusts { get; set; }
        public int PrecipitationProbability { get; set; }
        public double DeltaT { get; set; }
        public string Status { get; set; } = string.Empty; // "Apto", "Precaucion", "NoApto"
        public string Color { get; set; } = string.Empty;  // "green", "yellow", "red"
        public string Reason { get; set; } = string.Empty; // Explicación técnica para el aplicador
    }

    public class AgroAlertDto
    {
        public string Type { get; set; } = string.Empty;     // "HeladaAgronomica", "SueloFrio", "RiesgoCosecha", etc.
        public string Severity { get; set; } = string.Empty; // "Info", "Warning", "Critical"
        public string Message { get; set; } = string.Empty;
        public DateTime? TargetDate { get; set; }
    }

    public class SoilAndWaterBalanceDto
    {
        public double CurrentSoilTemp6cm { get; set; }
        public double CurrentSoilMoisture { get; set; }
        public bool IsSoilReadyForSowing { get; set; }
        public string SoilAdvice { get; set; } = string.Empty;
        public double Next5DaysRainMm { get; set; }
        public double Next5DaysEvapotranspirationMm { get; set; }
        public double NetWaterBalanceMm { get; set; } // Lluvia - Evapotranspiración
    }

    public class LotDashboardDto
    {
        public FieldLotDto Lot { get; set; } = null!;
        public HourlyWeatherDto? CurrentWeather { get; set; }
        public SoilAndWaterBalanceDto SoilAndWater { get; set; } = new();
        public List<AgroAlertDto> ActiveAlerts { get; set; } = new();
        public List<SprayingHourDto> SprayingWindow48h { get; set; } = new();
        public List<DailyWeatherDto> DailyForecast { get; set; } = new();
    }
}
