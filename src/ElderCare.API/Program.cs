using System.Text;
using ElderCare.Application;
using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Services;
using ElderCare.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .CreateLogger();

//builder.Host.UseSerilog();

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ElderCare Connect API",
        Version = "v1",
        Description = "API for ElderCare Connect Platform"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
});

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// SignalR
builder.Services.AddSignalR();

// HttpContextAccessor (required for CurrentUserService)
builder.Services.AddHttpContextAccessor();

// Application & Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IMatchingService, MatchingService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICaregiverAssistantService, MLNetCaregiverAssistantService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IFraudDetectionService, FraudDetectionService>();
builder.Services.AddHttpClient<IChatbotService, ChatbotService>();



var app = builder.Build();

// Seed sample data (only when DB is empty)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ElderCare.Infrastructure.Persistence.ElderCareDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<ElderCare.Application.Common.Interfaces.IPasswordHasher>();
    await ElderCare.Infrastructure.Persistence.SeedData.SeedAsync(context, passwordHasher);
}

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ElderCare Connect API V1");
    });
}

//app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Serve SPA static files from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

// Map SignalR hubs
app.MapHub<ElderCare.API.Hubs.NotificationHub>("/hubs/notifications");
app.MapHub<ElderCare.API.Hubs.ChatHub>("/hubs/chat");

// SPA fallback — client-side routing support
app.MapFallbackToFile("index.html");

app.Run();


