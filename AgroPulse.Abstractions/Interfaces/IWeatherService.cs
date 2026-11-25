using AgroPulse.Abstractions.DTOs.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroPulse.Abstractions.Interfaces
{
    public interface IWeatherService
    {
        Task<AgroForecastDto?> GetAgroForecastAsync(decimal latitude, decimal longitude);
    }
}
