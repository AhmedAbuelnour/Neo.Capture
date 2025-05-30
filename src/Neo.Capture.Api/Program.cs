using LowCodeHub.MinimalEndpoints.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Neo.Capture.Application.Interfaces.Repositories;
using Neo.Capture.Application.Interfaces.Services;
using Neo.Capture.Application.Providers;
using Neo.Capture.Domain.Entities;
using Neo.Capture.Infrastructure;
using Neo.Capture.Infrastructure.Implementations.Repositories;
using Neo.Capture.Infrastructure.Implementations.Services;
using Neo.Common.UserProvider;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi("neo-capture", options =>
{
    // Only include endpoints whose ApiExplorer GroupName matches
    // options.ShouldInclude = apiDesc => apiDesc.GroupName == "dmc-module";

    options.AddDocumentTransformer((doc, ctx, ct) =>
    {
        doc.Servers.Clear();

        //doc.Servers.Add(new OpenApiServer
        //{
        //    Url = "/"
        //});

        doc.Info = new()
        {
            Title = "Dmc API",
            Version = "v1"
        };

        doc.Components ??= new OpenApiComponents();

        // Add security definition for JWT Bearer authentication
        doc.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });

        doc.SecurityRequirements ??= [];

        // Add global security requirement
        doc.SecurityRequirements.Add(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });

        return Task.CompletedTask;
    });
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.RequireHttpsMetadata = false; // for dev only
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "Neo.Capture",

            ValidateAudience = true,           // set false if you don't care about audience
            ValidAudience = "Neo.Capture",

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2),

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F"))
        };
    });
builder.Services.AddScoped<IPasswordHasher<Profile>, PasswordHasher<Profile>>();

builder.Services.AddScoped<ICloudStorageService, GoogleCloudStorageService>();

builder.Services.AddScoped<JwtTokenProvider>();

builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ILocationService, LocationService>();

builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddValidators<Program>();

//builder.Services.AddDbContext<NeoCaptureDbContext>(options =>
//{
//    // use postgresql for the database connection string
//    options.UseNpgsql(builder.Configuration.GetConnectionString("NeoCaptureDb"));
//});

builder.Services.AddDbContext<NeoCaptureDbContext>(options =>
{
    // use in-memory database for development purposes
    options.UseInMemoryDatabase("NeoCaptureDb");
});

builder.Services.AddModules<Program>();

builder.Services.AddEndpoints<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/neo-capture.json", "Neo Capture API");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication().UseAuthorization();

app.MapModules();

app.Run();

