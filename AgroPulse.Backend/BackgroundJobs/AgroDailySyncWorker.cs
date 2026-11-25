using AgroPulse.Abstractions.Interfaces;

namespace AgroPulse.Backend.BackgroundJobs
{
    public class AgroDailySyncWorker(IServiceScopeFactory _scopeFactory, ILogger<AgroDailySyncWorker> _logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Iniciando sincronización agrometeorológica en segundo plano...");
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var lotRepo = scope.ServiceProvider.GetRequiredService<IFieldLotRepository>();
                    var weatherService = scope.ServiceProvider.GetRequiredService<IWeatherService>();
                    var rainRepo = scope.ServiceProvider.GetRequiredService<IRainRecordRepository>();

                    var allLots = await lotRepo.GetAllLostAsync();
                    var today = DateOnly.FromDateTime(DateTime.Today);

                    foreach (var lot in allLots) 
                    {
                        if (stoppingToken.IsCancellationRequested) break;
                        
                        var forecast = await weatherService.GetAgroForecastAsync(lot.Latitude, lot.Longitude);
                        if (forecast is null) continue;

                        var pastRainDays = forecast.Daily
                                .Where(d => d.Date < today && d.PrecipitationSum > 0.2);

                        foreach( var day in pastRainDays)
                        {
                            await rainRepo.SyncSatellitalRecordAsync(lot.Id, day.Date, (decimal)day.PrecipitationSum);
                        }

                        _logger.LogInformation("Sincronización completada para {Count} lotes.", allLots.Count);
                    }

                }
                catch (Exception ex) 
                {
                    _logger.LogError(ex, "Error durante la sincronización diaria en segundo plano.");
                }

                await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
            }
        }
    }
}
