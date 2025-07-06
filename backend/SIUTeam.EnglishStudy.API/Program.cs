using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SIUTeam.EnglishStudy.API.Hubs;
using SIUTeam.EnglishStudy.API.Mapping;
using SIUTeam.EnglishStudy.Infrastructure.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
IdentityModelEventSource.ShowPII = true;
// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtKey = jwtSettings["Secret"];
var jwtIssuer = jwtSettings["Issuer"];
var jwtAudience = jwtSettings["Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new ArgumentException("JWT secret key must be at least 32 characters long.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero,
        NameClaimType = ClaimTypes.Name
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = x =>
        {
            Console.WriteLine("Token validated successfully.");
            var jwtToken = x.SecurityToken as JwtSecurityToken;
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = x =>
        {
            Console.WriteLine($"Authentication failed: {x.Exception}");
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            var token = context.Request.Query["access_token"];
            if (context.Request.Path.StartsWithSegments("/hubs") && string.IsNullOrEmpty(context.Token))
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                }
                else if (!string.IsNullOrWhiteSpace(token))
                {
                    context.Token = token;
                }
            }
            return Task.CompletedTask;
        }
    };
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "SIU Team English Study API",
        Description = "API for English Study Application with JWT Authentication and Role-based Authorization",
        Contact = new OpenApiContact
        {
            Name = "SIU Team",
            Email = "contact@siuteam.com",
            Url = new Uri("https://siuteam.com")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML comments for better documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Enable annotations
    c.EnableAnnotations();

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
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
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3001", "https://localhost:5174", "http://localhost:5073") // Add your frontend URLs
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for SignalR
    });
});

// Add SignalR
builder.Services.AddSignalR();

// Register Infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add Mapster mapping profiles
builder.Services.AddMappingProfiles();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SIU Team English Study API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Enable CORS - must be before UseHttpsRedirection for development
app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map SignalR hubs - Use flexible authentication policy
app.MapHub<SpeakingHub>("/hubs/speaking").RequireAuthorization();

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
