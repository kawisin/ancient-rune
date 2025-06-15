using AspNetCoreRateLimit;
using Instance_Service.DB;
using Instance_Service.Services;
using Shares.Helper;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<InstanceManagerService>();

//RateLimit
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


//var envPath = Path.Combine(AppContext.BaseDirectory, "Shares", ".env");
if (Environment.GetEnvironmentVariable("DOCKER_ON") != "true")
{
    var envPath = Path.GetFullPath(@"..\Shares\.env");
    Env.Load(envPath);
}
//
//
////AUTH
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = issuer,
//        ValidAudience = audience,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
//    };
//});
//
//builder.Services.AddAuthorization();

//if (builder.Environment.IsProduction())
//{
//    builder.WebHost.UseUrls("http://localhost:5251");
//}

//builder.WebHost.UseUrls("http://localhost:5251");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var lifetime = app.Lifetime;

lifetime.ApplicationStopping.Register(() =>
{
    try
    {
        //Shares.Logger.Info("Shutting down...");
        //Shares.Logger.Info("Shutting down...");
        CleanupService.CleanupBeforeShutdown();
    }
    catch (Exception ex)
    {
        Logger.Info($"Shutdown cleanup error: {ex.Message}");
        //Shares.Logger.Info($"Shutdown cleanup error: {ex.Message}");
    }
});

Logger.Info($"Instance Service - Started");
app.Run();
