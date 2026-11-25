using AgroPulse.Abstractions.Interfaces;
using AgroPulse.Backend.BackgroundJobs;
using AgroPulse.Core.Services;
using AgroPulse.Data.EF;
using AgroPulse.Data.ExternalServices.OpenMeteo;
using AgroPulse.Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AgroPulseDBContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MYSQL_CONNECTION_STRING"),
        new MySqlServerVersion(new Version(8, 0, 32)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(3) // 3 intentos
        )
);

//Habilitamos cache
builder.Services.AddMemoryCache();
// Registrar HttpClient tipado para Open-Meteo
builder.Services.AddHttpClient<IWeatherService, OpenMeteoService>(client =>
{
    client.BaseAddress = new Uri("https://api.open-meteo.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

//Repository
builder.Services.AddScoped<IFieldLotRepository, FieldLotRepository>();
builder.Services.AddScoped<IRainRecordRepository, RainRecordRepository>();

//Core
builder.Services.AddScoped<IAgroAnalyticsService, AgroAnalyticsService>();
builder.Services.AddScoped<IFieldLotService, FieldLotService>();
builder.Services.AddScoped<IRainRecordService, RainRecordService>();

//Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

//Worker
builder.Services.AddHostedService<AgroDailySyncWorker>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
