using SIUTeam.EnglishStudy.Infrastructure.Extensions;
using SIUTeam.EnglishStudy.Infrastructure.Authorization;
using SIUTeam.EnglishStudy.Core.Authorization;
using SIUTeam.EnglishStudy.Core.Enums;
using SIUTeam.EnglishStudy.Core.Entities;
using SIUTeam.EnglishStudy.API.Mapping;
using SIUTeam.EnglishStudy.API.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? "SIUTeamEnglishStudySecretKey2024!@#$%^&*()";
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false; // Set to true in production
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
    
    // Configure JWT for SignalR
    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            // Skip JWT authentication challenge for SignalR hubs
            var path = context.HttpContext.Request.Path;
            if (path.StartsWithSegments("/hubs"))
            {
                context.HandleResponse();
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }
    };
});

// Configure Authorization with custom policies
builder.Services.AddAuthorization(options =>
{
    // Add permission-based policies
    foreach (Permission permission in Enum.GetValues<Permission>())
    {
        options.AddPolicy($"Permission.{permission}", policy =>
            policy.Requirements.Add(new PermissionRequirement(permission)));
    }

    // Add role-based policies
    foreach (UserRole role in Enum.GetValues<UserRole>())
    {
        options.AddPolicy($"Role.{role}", policy =>
            policy.Requirements.Add(new RoleRequirement(role)));
    }

    // Add default policies
    options.AddPolicy("RequireAuthentication", policy =>
        policy.RequireAuthenticatedUser());
    
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole(UserRole.Admin.ToString()));
    
    options.AddPolicy("TeacherOrAdmin", policy =>
        policy.RequireRole(UserRole.Teacher.ToString(), UserRole.Admin.ToString()));
        
    // Add policy for SignalR anonymous access
    options.AddPolicy("SignalRAnonymous", policy =>
        policy.RequireAssertion(_ => true)); // Always allow
});

// Register authorization handlers
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ResourceAuthorizationHandler>();

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
        policy.WithOrigins("http://localhost:3000", "https://localhost:3001", "https://localhost:5174") // Add your frontend URLs
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

// Custom middleware to bypass authentication for SignalR negotiate
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/hubs/speaking/negotiate"))
    {
        // Skip authentication for SignalR negotiate endpoint
        await next();
    }
    else
    {
        await next();
    }
});

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map SignalR hubs
app.MapHub<SpeakingHub>("/hubs/speaking").AllowAnonymous();

app.Run();
