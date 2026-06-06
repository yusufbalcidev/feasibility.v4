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
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

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
