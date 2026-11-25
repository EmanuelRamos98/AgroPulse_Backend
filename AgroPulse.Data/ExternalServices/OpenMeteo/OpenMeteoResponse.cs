using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AgroPulse.Data.ExternalServices.OpenMeteo
{
    public class OpenMeteoResponse
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("hourly")]
        public HourlyData Hourly { get; set; } = new();

        [JsonPropertyName("daily")]
        public DailyData Daily { get; set; } = new();
    }

    public class HourlyData
    {
        [JsonPropertyName("time")]
        public List<string> Time { get; set; } = new();

        [JsonPropertyName("temperature_2m")]
        public List<double> Temperature2m { get; set; } = new();

        [JsonPropertyName("relative_humidity_2m")]
        public List<int> RelativeHumidity2m { get; set; } = new();

        [JsonPropertyName("precipitation_probability")]
        public List<int> PrecipitationProbability { get; set; } = new();

        [JsonPropertyName("precipitation")]
        public List<double> Precipitation { get; set; } = new();

        [JsonPropertyName("wind_speed_10m")]
        public List<double> WindSpeed10m { get; set; } = new();

        [JsonPropertyName("wind_gusts_10m")]
        public List<double> WindGusts10m { get; set; } = new();

        [JsonPropertyName("soil_temperature_6cm")]
        public List<double> SoilTemperature6cm { get; set; } = new();

        [JsonPropertyName("soil_moisture_3_to_9cm")]
        public List<double> SoilMoisture3To9cm { get; set; } = new();
    }

    public class DailyData
    {
        [JsonPropertyName("time")]
        public List<string> Time { get; set; } = new();

        [JsonPropertyName("et0_fao_evapotranspiration")]
        public List<double> Evapotranspiration { get; set; } = new();

        [JsonPropertyName("precipitation_sum")]
        public List<double> PrecipitationSum { get; set; } = new();

        [JsonPropertyName("temperature_2m_min")]
        public List<double> TemperatureMin { get; set; } = new();

        [JsonPropertyName("temperature_2m_max")]
        public List<double> TemperatureMax { get; set; } = new();
    }
}
