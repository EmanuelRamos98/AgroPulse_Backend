using AgroPulse.Abstractions.Interfaces;
using AgroPulse.Backend.BackgroundJobs;
using AgroPulse.Backend.Middleware;
using AgroPulse.Core.Services;
using AgroPulse.Data.EF;
using AgroPulse.Data.ExternalServices.OpenMeteo;
using AgroPulse.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

//Middleware
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

//DB
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
builder.Services.AddScoped<IUserRepository, UserRepository>();

//Core
builder.Services.AddScoped<IAgroAnalyticsService, AgroAnalyticsService>();
builder.Services.AddScoped<IFieldLotService, FieldLotService>();
builder.Services.AddScoped<IRainRecordService, RainRecordService>();
builder.Services.AddScoped<IAuthService, AuthService>();

//JWT
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

//Swagger Authorize
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AgroPulse API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Pegá tu token JWT así: Bearer {tu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

//Worker
builder.Services.AddHostedService<AgroDailySyncWorker>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
