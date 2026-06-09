using System.Reflection;
using System.Text;
using feasibility.Business.Abstract;
using feasibility.Business.Concrete;
using Microsoft.Extensions.Caching.Memory;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Enums;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace feasibility.Business.Extensions;

public static class BusinessServiceRegistration
{
    public static IServiceCollection AddBusiness(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddHttpClient<ITcmbService, TcmbService>();
        services.AddHttpClient<IWorldBankService, WorldBankService>();
        services.AddScoped<IFeasibilityAmortizationService, FeasibilityAmortizationManager>();
        services.AddScoped(typeof(IGenericService<>), typeof(GenericManager<>));
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IActivityLogService, ActivityLogManager>();
        services.AddScoped<IRolePermissionService, RolePermissionManager>();

        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(Assembly.GetExecutingAssembly());
            cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzk4MjQzMjAwIiwiaWF0IjoiMTc2Njc4NzA5OSIsImFjY291bnRfaWQiOiIwMTliNWNiNzNjMmE3NGIwODFlZGQzY2I1NmMzYTQzMCIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa2RlYmZhajI1cDV3OGUxMnJtYWc0ZGFwIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.g-UExFSz7Q0Xbl6cCCDPHz4vmK_iE6XC5SK9mkNO3-LpPP5FzfytzRnZ57BiagM1Op3d11datHmUkRjW4G-hSlcvVdeDP2iPTx5xNVWJUd4Kj2k24FWs4AZpojHK3hcu10KdZWPO_wzVs3pbVYGBlF9ZaeGdISzKxlkDxfNwFNF1lKgSqlyVwNaYXO9qiR7ZZ_-FbMV9wV9kU6pDElmAaN_CUpr6bh89adWZISdyYO2nvnnjW1z6e40yUmw9VJBCcRvrBsmI_fCnJLBX6l-mYjOg7rbaNQwuvfuFRbYlWaWdPxfDkD_-3qZqeU-QqEphWU2XrbiXSH1DHkAyXcvm7w";
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        // AutoValidation opt-in moduna alındı. Varsayılan (All) modda global filtre,
        // view controller'larında doğrulama hatasında View yerine 400 JSON ProblemDetails
        // döndürdüğü için Login gibi sayfalarda Türkçe uyarı yerine ham JSON görünüyordu.
        // Annotations stratejisiyle yalnızca [AutoValidation] attribute'u taşıyan
        // JSON/API action'ları otomatik doğrulanır; view controller'ları kendi
        // ModelState kontrolüyle (if (!ModelState.IsValid) return View(dto)) çalışır.
        services.AddFluentValidationAutoValidation(config =>
            config.ValidationStrategy = ValidationStrategy.Annotations);

        services.AddJwtAuthentication(configuration);
        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JwtSettings:SecretKey ayarı bulunamadı.");
        var key = Encoding.ASCII.GetBytes(secretKey);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["feasibility_jwt"];
                        if (!string.IsNullOrEmpty(token))
                            context.Token = token;
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/Login/Index");
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}
