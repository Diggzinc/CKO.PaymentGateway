using System.Text;
using System.Text.Json;
using AcquiringBank.Api.Client;
using CKO.PaymentGateway.Api.ViewModels;
using CKO.PaymentGateway.Host.Api.Constants;
using CKO.PaymentGateway.Services;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

var builder = WebApplication.CreateBuilder(args);

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
        // The symmetric key used is the default one for https://jwt.io (ease of use)
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("your-256-bit-secret")),
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

// Add AcquiringBank Client
builder.Services
    .AddTransient<IAcquiringBankClient>(provider =>
    {
        var endpoint = "http://localhost:3001";
        var apiKey = "some-api-key";
        return new AcquiringBankApiClient(endpoint, apiKey);
    });

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

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
    .AddAutoMapper(
        typeof(ApiViewModelsAnchor));

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
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
