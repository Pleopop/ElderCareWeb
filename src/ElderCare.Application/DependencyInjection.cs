using System.Reflection;
using ElderCare.Application.Common.Behaviors;
using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ElderCare.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Services
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IMatchingService, MatchingService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<INotificationService, NotificationService>();
        
        // Memory Cache
        services.AddMemoryCache();

        return services;
    }
}
