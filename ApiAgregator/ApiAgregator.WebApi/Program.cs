using ApiAgregator.BackgroundService.ExternalApis;
using ApiAgregator.Data;
using ApiAgregator.ExternalApi.ChuckNorris;
using ApiAgregator.ExternalApi.CovidApi;
using ApiAgregator.ExternalApi.WeatherApi;
using ApiAgregator.Services;
using ApiAgregator.Services.APIs;
using ApiAgregator.Services.Repositories;
using ApiAgregator.WebApi.ServiceCollectionExtensions;
using ApiAgregator.WebApi.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("conf.json");

var authJwtKey = builder.Configuration.GetValue<string>("authJwtKey");

// Add services to the container.

builder.Services.AddCors();

builder.Services.AddJwtService(options =>
{
    options.Key = authJwtKey;
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = JwtService.CreateTokenValidationParameters(authJwtKey);
    });

builder.Services
    .AddAuthorization(options =>
    {
        options.AddPolicy("user", policy =>
        {
            policy.RequireClaim(ClaimTypes.Email);
        });
        options.AddPolicy("admin", policy =>
        {
            policy.RequireRole("admin");
            policy.RequireClaim(ClaimTypes.Email);
        });
    });

builder.Services.AddControllers();

builder.Services.AddScoped<DBConnection>();

builder.Services.Configure<DBConnectionOption>((opt) =>
{
    opt.ConnectionString = builder.Configuration.GetValue<string>("dbConnection");
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICronTaskRepository, CronTaskRepository>();

builder.Services.AddSingleton(new SaltService(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("salt")), 16));

builder.Services.AddScoped<AuthenticationService>();

builder.Services.AddSingleton<IEmailSenderService, EmailSenderService>();
builder.Services.Configure((EmailSenderServiceOptions options) =>
{
    options.SmtpPort = builder.Configuration.GetValue<int>("smtpPort");
    options.SmtpHost = builder.Configuration.GetValue<string>("smtpHost");
    options.Credential = new System.Net.NetworkCredential(
        builder.Configuration.GetValue<string>("email"),
        builder.Configuration.GetValue<string>("emailPass")
    );
    options.From = builder.Configuration.GetValue<string>("email");
});

builder.Services.AddEmailValidationService(options =>
{
    options.Url = builder.Configuration.GetValue<string>("emailConfirmUrl");
    options.Key = builder.Configuration.GetValue<string>("emailConfirmKey");
});

builder.Services.AddCronTaskScheduler(options =>
{
    options.ThreadCount = builder.Configuration.GetValue<int>("threadCount");
});

builder.Services.AddScoped<ExternalApisService>();

builder.Services.AddScoped<IExternalApi, ChuckNorrisApi>();

builder.Services.AddScoped<IExternalApi, WeatherApi>();
builder.Services.Configure((WeatherApiOptions options) =>
{
    options.Key = builder.Configuration.GetValue<string>("weatherApiKey");
});

builder.Services.AddScoped<IExternalApi, CovidApi>();
builder.Services.Configure((CovidApiOptions options) =>
{
    options.XRapidApiKey = builder.Configuration.GetValue<string>("XRapidApiKey");
    options.XRapidApiHost = builder.Configuration.GetValue<string>("XRapidApiHost");
});

var app = builder.Build();

app.UseCors(configurePolicy => configurePolicy
    .WithOrigins(builder.Configuration.GetValue<string>("webClient"))
    .AllowAnyHeader()
    .AllowAnyMethod());

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var eas = scope.ServiceProvider.GetService<ExternalApisService>()!;
    eas.StartAll();
}

app.Run();

