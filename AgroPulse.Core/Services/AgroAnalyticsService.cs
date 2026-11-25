using AgroPulse.Abstractions.DTOs.Analytics;
using AgroPulse.Abstractions.DTOs.Lost;
using AgroPulse.Abstractions.DTOs.Weather;
using AgroPulse.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Core.Services
{
    public class AgroAnalyticsService(IFieldLotRepository _lotRepository, IWeatherService _weatherService, IRainRecordRepository _rainRepository) : IAgroAnalyticsService
    {
        public async Task<LotDashboardDto?> GetLotDashboardDtoAsync(int lotId, int userId)
        {
            var lot = await _lotRepository.GetByIdAsync(lotId, userId);
            if(lot is null) return null;

            var forecast = await _weatherService.GetAgroForecastAsync(lot.Latitude, lot.Longitude);
            if (forecast is null)
            {
                return new LotDashboardDto { Lot = lot };
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            //Guardamos automaticamente lluvias en dias pasados siempre que sean > 0.2 mm
            var pastRainDays = forecast.Daily
                    .Where(d => d.Date < today && d.PrecipitationSum > 0.2)
                    .ToList();

            foreach (var day in pastRainDays)
            {
                await _rainRepository.SyncSatellitalRecordAsync(lot.Id, day.Date, (decimal)day.PrecipitationSum);
            }

            var now = DateTime.Now;
            var upcomingHours = forecast.Hourly.Where(h => h.Time >= now.AddHours(-1)).ToList();
            var futureDaily = forecast.Daily.Where(d => d.Date >= today).ToList();
            var currentHour = upcomingHours.FirstOrDefault() ?? forecast.Hourly.FirstOrDefault();

            //Calcular ventana de pulverizacion 48 horas
            var sprayingWindow = upcomingHours.Take(48).Select(EvaluateSprayingHour).ToList();

            //Calcular el balance hidrico y estado del suelo
            var soilAndWater = EvaluateSoilAndWaterBalance(lot, upcomingHours, forecast.Daily);

            //Generar alertas tempranas contextualizadas al cultivo y etapa
            var alerts = GenerateAgroAlerts(lot, upcomingHours, soilAndWater);

            return new LotDashboardDto
            {
                Lot = lot,
                CurrentWeather = currentHour,
                SoilAndWater = soilAndWater,
                ActiveAlerts = alerts,
                SprayingWindow48h = sprayingWindow,
                DailyForecast = futureDaily
            };
        }

        private static SprayingHourDto EvaluateSprayingHour(HourlyWeatherDto hour)
        {
            double deltaT = CalculateDeltaT(hour.Temperature, hour.RelativeHumidity);
            string status, color, reason;

            // Reglas estrictas de pulverización
            if (hour.PrecipitationProbability > 40 || hour.Precipitation > 0.2)
            {
                status = "NoApto";
                color = "red";
                reason = "Alto riesgo de lavado por lluvia.";
            }
            else if (hour.WindSpeed > 15 || hour.WindGusts > 22)
            {
                status = "NoApto";
                color = "red";
                reason = $"Viento excesivo ({hour.WindSpeed} km/h, ráfagas {hour.WindGusts} km/h): riesgo de deriva.";
            }
            else if (deltaT > 10.0)
            {
                status = "NoApto";
                color = "red";
                reason = $"Delta T muy alto ({deltaT:F1}): evaporación rápida de la gota.";
            }
            else if (deltaT < 2.0)
            {
                status = "NoApto";
                color = "red";
                reason = $"Delta T muy bajo ({deltaT:F1}): riesgo de inversión térmica (nube flotante).";
            }
            // Reglas de precaución
            else if (hour.WindSpeed >= 10 || deltaT >= 8.0)
            {
                status = "Precaucion";
                color = "yellow";
                reason = deltaT >= 8.0
                    ? $"Delta T en límite ({deltaT:F1}): usar coadyuvante antievaporante."
                    : $"Viento moderado ({hour.WindSpeed} km/h): usar pastillas antideriva.";
            }
            else
            {
                status = "Apto";
                color = "green";
                reason = $"Condiciones óptimas (Delta T: {deltaT:F1}, Viento: {hour.WindSpeed} km/h).";
            }

            return new SprayingHourDto
            {
                Time = hour.Time,
                Temperature = hour.Temperature,
                RelativeHumidity = hour.RelativeHumidity,
                WindSpeed = hour.WindSpeed,
                WindGusts = hour.WindGusts,
                PrecipitationProbability = hour.PrecipitationProbability,
                DeltaT = Math.Round(deltaT, 1),
                Status = status,
                Color = color,
                Reason = reason
            };
        }

        /// IMPORTANTE
        /// Calcula el Delta T = Temperatura bulbo seco - Temperatura bulbo húmedo (Fórmula de Stull, 2011)
        
        private static double CalculateDeltaT(double tempC, int rh)
        {
            double wetBulb = tempC * Math.Atan(0.151977 * Math.Sqrt(rh + 8.313659))
                       + Math.Atan(tempC + rh)
                       - Math.Atan(rh - 1.676331)
                       + 0.00391838 * Math.Pow(rh, 1.5) * Math.Atan(0.023101 * rh)
                       - 4.686035;

            return Math.Max(0, tempC - wetBulb);
        }

        private static SoilAndWaterBalanceDto EvaluateSoilAndWaterBalance(FieldLotDto lot, List<HourlyWeatherDto> upcomingHours, List<DailyWeatherDto> daily)
        {
            var current = upcomingHours.FirstOrDefault();
            double currentSoilTemp = current?.SoilTemperature6cm ?? 0;
            double currentSoilMoisture = current?.SoilMoisture3To9cm ?? 0;

            // Promedio de temperatura del suelo a 6cm en las próximas 72hs
            double avgSoilTemp72h = upcomingHours.Take(72).Any()
                ? upcomingHours.Take(72).Average(h => h.SoilTemperature6cm)
                : currentSoilTemp;

            // Cultivos estivales (Soja=1, Maiz=2, Girasol=4) requieren al menos 14-15°C de suelo
            bool isSummerCrop = lot.CropType is 1 or 2 or 4;
            double minRequiredTemp = isSummerCrop ? 14.5 : 8.0;

            bool isReady = avgSoilTemp72h >= minRequiredTemp && currentSoilMoisture >= 0.15;

            string advice = isReady
                ? $"Piso térmico y hídrico adecuado (Prom. 72hs: {avgSoilTemp72h:F1}°C)."
                : avgSoilTemp72h < minRequiredTemp
                    ? $"Suelo frío ({avgSoilTemp72h:F1}°C prom.): esperar a superar {minRequiredTemp}°C para evitar fallas de emergencia."
                    : "Baja humedad superficial en cama de siembra (< 15%).";

            double totalRain = daily.Sum(d => d.PrecipitationSum);
            double totalEt0 = daily.Sum(d => d.Evapotranspiration);

            return new SoilAndWaterBalanceDto
            {
                CurrentSoilTemp6cm = Math.Round(currentSoilTemp,1),
                CurrentSoilMoisture = Math.Round(currentSoilMoisture * 100),
                IsSoilReadyForSowing = isReady,
                SoilAdvice = advice,
                Next5DaysRainMm = Math.Round(totalRain, 1),
                Next5DaysEvapotranspirationMm = Math.Round(totalEt0, 1),
                NetWaterBalanceMm = Math.Round(totalRain - totalEt0, 1)
            };
        }

        private static List<AgroAlertDto> GenerateAgroAlerts(FieldLotDto lot, List<HourlyWeatherDto> upcomingHours, SoilAndWaterBalanceDto soilAndWater)
        {
            var alerts = new List<AgroAlertDto>();

            // Alerta de Helada Agronómica (Temp aire <= 3.0°C en próximas 72hs)
            var frostHour = upcomingHours.Take(72).FirstOrDefault(h => h.Temperature <= 3.0);
            if (frostHour is not null && lot.CropType != 0) // Si no es Barbecho
            {
                alerts.Add(new AgroAlertDto
                {
                    Type = "HeladaAgronomica",
                    Severity = frostHour.Temperature <= 1.0 ? "Critical" : "Warning",
                    Message = $"Riesgo de helada agronómica ({frostHour.Temperature:F1}°C en abrigo) sobre {lot.CropTypeName}.",
                    TargetDate = frostHour.Time
                });
            }

            // Alerta de Suelo Frío si el lote está en Pre-Siembra o Siembra
            if (lot.CropStage is 0 or 1 && !soilAndWater.IsSoilReadyForSowing)
            {
                alerts.Add(new AgroAlertDto
                {
                    Type = "SueloFrioSiembra",
                    Severity = "Warning",
                    Message = soilAndWater.SoilAdvice
                });
            }

            // Alerta de estrés hídrico en Floración Crítica (Stage = 3)
            if (lot.CropStage == 3 && soilAndWater.NetWaterBalanceMm < -15.0)
            {
                alerts.Add(new AgroAlertDto
                {
                    Type = "EstresHidrico",
                    Severity = "Critical",
                    Message = $"Déficit hídrico proyectado ({soilAndWater.NetWaterBalanceMm} mm) en etapa crítica de {lot.CropTypeName}."
                });
            }

            // Alerta de lluvia en Cosecha (Stage = 5)
            if (lot.CropStage == 5 && soilAndWater.Next5DaysRainMm > 10.0)
            {
                alerts.Add(new AgroAlertDto
                {
                    Type = "RiesgoCosecha",
                    Severity = "Warning",
                    Message = $"Se pronostican {soilAndWater.Next5DaysRainMm} mm en los próximos 5 días con el lote listo para cosechar."
                });
            }

            return alerts;
        }
    }
}
