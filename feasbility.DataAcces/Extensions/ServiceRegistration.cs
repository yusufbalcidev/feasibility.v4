using feasibility.DataAccess.Abstract;
using feasibility.DataAccess.Context;
using feasibility.DataAccess.Services;
using feasibility.Entity.Entities.Identity;
using feasibility.Entity.Validations.IdentityValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace feasibility.DataAccess.Extensions;

public static class ServiceRegistration
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection ayarı bulunamadı. " +
                "Local için 'dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"...\"' komutuyla tanımlayın.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null)));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = configuration.GetValue("IdentitySettings:Password:RequiredLength", 6);
                options.Password.RequireNonAlphanumeric = configuration.GetValue("IdentitySettings:Password:RequireNonAlphanumeric", false);
                options.Password.RequireLowercase = configuration.GetValue("IdentitySettings:Password:RequireLowercase", false);
                options.Password.RequireUppercase = configuration.GetValue("IdentitySettings:Password:RequireUppercase", false);
                options.Password.RequireDigit = configuration.GetValue("IdentitySettings:Password:RequireDigit", false);
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.User.RequireUniqueEmail = configuration.GetValue("IdentitySettings:User:RequireUniqueEmail", true);
                options.User.AllowedUserNameCharacters = configuration.GetValue("IdentitySettings:User:AllowedUserNameCharacters",
                    "abcçdefghiıjklmnoöpqrsştuüvwxyzABCÇDEFGHIİJKLMNOÖPQRSŞTUÜVWXYZ0123456789-._@+")!;
            })
            .AddErrorDescriber<CustomIdentityErrorDescriber>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
