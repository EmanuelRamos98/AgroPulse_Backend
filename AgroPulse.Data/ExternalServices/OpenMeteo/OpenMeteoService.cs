using AgroPulse.Abstractions.DTOs.Weather;
using AgroPulse.Abstractions.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AgroPulse.Data.ExternalServices.OpenMeteo
{
    public class OpenMeteoService(HttpClient _httpClient, IMemoryCache _cache, ILogger<OpenMeteoService> _logger) : IWeatherService
    {
        public async Task<AgroForecastDto?> GetAgroForecastAsync(decimal latitude, decimal longitude)
        {
            //2 decimales para la clave de caché (~1.1 km de precisión)
            string cacheKey = $"weather_{Math.Round(latitude, 2)}_{Math.Round(longitude, 2)}";

            if (_cache.TryGetValue(cacheKey, out AgroForecastDto? cachedData))
            {
                return cachedData;
            }

            string latStr = latitude.ToString(CultureInfo.InvariantCulture);
            string lonStr = longitude.ToString(CultureInfo.InvariantCulture);

            string url = $"v1/forecast?latitude={latStr}&longitude={lonStr}" +
                            "&hourly=temperature_2m,relative_humidity_2m,precipitation_probability,precipitation,wind_speed_10m,wind_gusts_10m,soil_temperature_6cm,soil_moisture_3_to_9cm" +
                            "&daily=et0_fao_evapotranspiration,precipitation_sum,temperature_2m_min,temperature_2m_max" +
                            "&timezone=America%2FArgentina%2FBuenos_Aires&past_days=30&forecast_days=5";

            try {
                var rawResponse = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(url);
                if (rawResponse == null) return null;

                var dto = new AgroForecastDto
                {
                    Latitude = (decimal)rawResponse.Latitude,
                    Longitude = (decimal)rawResponse.Longitude
                };

                for (int i = 0; i < rawResponse.Hourly.Time.Count; i++)
                {
                    dto.Hourly.Add(new HourlyWeatherDto
                    {
                        Time = DateTime.Parse(rawResponse.Hourly.Time[i]),
                        Temperature = rawResponse.Hourly.Temperature2m[i],
                        RelativeHumidity = rawResponse.Hourly.RelativeHumidity2m[i],
                        PrecipitationProbability = rawResponse.Hourly.PrecipitationProbability[i],
                        Precipitation = rawResponse.Hourly.Precipitation[i],
                        WindSpeed = rawResponse.Hourly.WindSpeed10m[i],
                        WindGusts = rawResponse.Hourly.WindGusts10m[i],
                        SoilTemperature6cm = rawResponse.Hourly.SoilTemperature6cm[i],
                        SoilMoisture3To9cm = rawResponse.Hourly.SoilMoisture3To9cm[i]
                    });
                }

                for (int i = 0; i < rawResponse.Daily.Time.Count; i++)
                {
                    dto.Daily.Add(new DailyWeatherDto
                    {
                        Date = DateOnly.Parse(rawResponse.Daily.Time[i]),
                        Evapotranspiration = rawResponse.Daily.Evapotranspiration[i],
                        PrecipitationSum = rawResponse.Daily.PrecipitationSum[i],
                        TemperatureMin = rawResponse.Daily.TemperatureMin[i],
                        TemperatureMax = rawResponse.Daily.TemperatureMax[i],
                    });
                }

                _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(30));
                return dto;
            }
            catch (HttpRequestException _ex)
            {
                _logger.LogError(_ex, "Error de red al consultar Open-Meteo para Lat: {Lat}, Lon: {Lon}", latitude, longitude);
                return null;
            }
            catch (TaskCanceledException _ex)
            {
                _logger.LogWarning(_ex, "Timeout al consultar Open-Meteo para Lat: {Lat}, Lon: {Lon}", latitude, longitude);
                return null;
            }
            catch(Exception _ex)
            {
                _logger.LogError(_ex, "Error inesperado al procesar la respuesta de Open-Meteo");
                return null;
            }
        }

    }

}
