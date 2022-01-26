using System.Text;
using System.Text.Json;
using AcquiringBank.Api.Client;
using CKO.PaymentGateway.Api.ViewModels;
using CKO.PaymentGateway.Host.Api.Configurations;
using CKO.PaymentGateway.Host.Api.Constants;
using CKO.PaymentGateway.Host.Api.HealthChecks;
using CKO.PaymentGateway.Services;
using CKO.PaymentGateway.Services.Abstractions.Errors;
using CKO.PaymentGateway.Services.Abstractions.Requests;
using CKO.PaymentGateway.Services.Abstractions.Responses;
using CKO.PaymentGateway.Services.Entities;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using OneOf;
using RockLib.HealthChecks.AspNetCore.ResponseWriter;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var configuration = new PaymentGatewayApiConfiguration(
    builder.Configuration.GetValue<string>(EnvironmentVariable.IssuerKey),
    builder.Configuration.GetValue<string>(EnvironmentVariable.AcquiringBankApiEndpoint),
    builder.Configuration.GetValue<string>(EnvironmentVariable.AcquiringBankApiKey),
    builder.Configuration.GetValue<string>(EnvironmentVariable.ConnectionString));

builder.Services.AddSingleton(configuration);

// Add services to the container.
builder.Services.AddHttpContextAccessor();

// Add authentication and authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PaymentGatewayPolicy.MerchantOnly, policy => policy.RequireClaim(PaymentGatewayClaim.MerchantId));
});
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration.IssuerKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = false,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.FromDays(1),
        };
    });

// Add API versioning.
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
});

// Add Database
builder.Services.AddDbContext<PaymentGatewayContext>(options =>
    options
        .UseNpgsql(configuration.ConnectionString)
        .UseSnakeCaseNamingConvention());

// Add AcquiringBank Client
builder.Services
    .AddTransient<IAcquiringBankClient>(provider =>
    {
        var endpoint = configuration.AcquiringBankApiEndpoint;
        var apiKey = configuration.AcquiringBankApiKey;
        return new AcquiringBankApiClient(endpoint, apiKey);
    });

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

// Add health checks
builder.Services
    .AddHealthChecks()
    .AddCheck<PostgresHealthCheck>("data-storage", HealthStatus.Unhealthy, timeout: TimeSpan.FromSeconds(2))
    .AddCheck<AcquiringBankHealthCheck>("acquiring-bank-services", HealthStatus.Degraded, timeout: TimeSpan.FromSeconds(5));

// Add logging
Log.Logger = new LoggerConfiguration()
    .Enrich.WithExceptionDetails()
    .Enrich.FromLogContext()
    .Enrich.WithAssemblyName()
    .Enrich.WithAssemblyVersion()
    .Enrich.WithClientIp()
    .Enrich.WithClientAgent()
        .WriteTo.Console()
        .MinimumLevel.Is(LogEventLevel.Information)
        .MinimumLevel.Override(LoggerSource.ExceptionHandlerMiddleware, LogEventLevel.Fatal)
    .CreateLogger();
;
;

builder.Host.UseSerilog();
builder.Services
    .AddLogging(provider =>
    {
        provider.ClearProviders();
        provider.AddSerilog();
    });

builder.Services
    .AddFluentValidation(config =>
    {
        config.RegisterValidatorsFromAssemblyContaining<ApiViewModelsAnchor>();

        // Fix for property names or errors to comply with the JsonNamingPolicy.
        static string ToPascalCase(string name) => char.ToLowerInvariant(name[0]) + name[1..];
        config.ValidatorOptions.PropertyNameResolver =
            (_, member, _) => ToPascalCase(member.Name);
        config.ValidatorOptions.DisplayNameResolver =
            (_, member, _) => ToPascalCase(member.Name);
    });

builder.Services
    .AddMediatR(typeof(PaymentGatewayServicesAnchor));
builder.Services
    .AddTransient<
        IPipelineBehavior<ProcessPaymentRequest, OneOf<PaymentServiceError, ProcessPaymentResponse>>,
        ProcessPaymentRequestLoggingBehavior
    >();

builder.Services
    .AddAutoMapper(
        typeof(ApiViewModelsAnchor),
        typeof(PaymentGatewayServicesAnchor));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = RockLibHealthChecks.ResponseWriter
});
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
