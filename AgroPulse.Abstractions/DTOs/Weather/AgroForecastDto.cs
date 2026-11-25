using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AgroPulse.Abstractions.DTOs.Weather
{
    public class AgroForecastDto
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public List<HourlyWeatherDto> Hourly { get; set; } = new();
        public List<DailyWeatherDto> Daily { get; set; } = new();
    }

    public class HourlyWeatherDto
    {
        public DateTime Time { get; set; }
        public double Temperature { get; set; }
        public int RelativeHumidity { get; set; }
        public int PrecipitationProbability { get; set; }
        public double Precipitation { get; set; }
        public double WindSpeed { get; set; }
        public double WindGusts { get; set; }
        public double SoilTemperature6cm { get; set; }
        public double SoilMoisture3To9cm { get; set; }
    }

    public class DailyWeatherDto
    {
        public DateOnly Date { get; set; }
        public double Evapotranspiration { get; set; }
        public double PrecipitationSum { get; set; }
        public double TemperatureMin { get; set; }
        public double TemperatureMax { get; set; }
    }
}
