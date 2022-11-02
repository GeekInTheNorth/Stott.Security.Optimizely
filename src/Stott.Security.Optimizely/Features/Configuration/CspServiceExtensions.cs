namespace Stott.Security.Optimizely.Features.Configuration;

using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Audit;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Header;
using Stott.Security.Optimizely.Features.Middleware;
using Stott.Security.Optimizely.Features.Permissions.List;
using Stott.Security.Optimizely.Features.Permissions.Repository;
using Stott.Security.Optimizely.Features.Permissions.Service;
using Stott.Security.Optimizely.Features.Reporting.Repository;
using Stott.Security.Optimizely.Features.SecurityHeaders.Repository;
using Stott.Security.Optimizely.Features.SecurityHeaders.Service;
using Stott.Security.Optimizely.Features.Settings.Repository;
using Stott.Security.Optimizely.Features.Settings.Service;
using Stott.Security.Optimizely.Features.Whitelist;

public static class CspServiceExtensions
{
    public static IServiceCollection AddCspManager(
        this IServiceCollection services, 
        Action<CspSetupOptions> cspSetupOptions = null, 
        Action<AuthorizationOptions> authorizationOptions = null)
    {
        var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

        // Handle null CSP Setup Options.
        var concreteOptions = new CspSetupOptions();
        if (cspSetupOptions != null)
        {
            cspSetupOptions(concreteOptions);
        }
        else
        {
            concreteOptions.ConnectionStringName = "EPiServerDB";
        }

        // Service Dependencies
        services.SetUpCspDependencies();

        // Authorization
        if (authorizationOptions != null)
        {
            services.AddAuthorization(authorizationOptions);
        }
        else
        {
            var allowedRoles = new List<string> { "CmsAdmins", "Administrator", "WebAdmins" };
            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy(CspConstants.AuthorizationPolicy, policy =>
                {
                    policy.RequireRole(allowedRoles);
                });
            });
        }

        // Database
        var connectionStringName = string.IsNullOrWhiteSpace(concreteOptions.ConnectionStringName) ? "EPiServerDB" : concreteOptions.ConnectionStringName;
        var connectionString = configuration.GetConnectionString(connectionStringName);
        services.SetUpCspDatabase(connectionString);

        return services;
    }

    public static void UseCspManager(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<SecurityHeaderMiddleware>();

        using var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetService<CspDataContext>();
        context.Database.Migrate();
    }

    internal static void SetUpCspDependencies(this IServiceCollection services)
    {
        services.AddTransient<ICspPermissionsListModelBuilder, CspPermissionsListModelBuilder>();
        services.AddTransient<ICspPermissionRepository, CspPermissionRepository>();
        services.AddTransient<ICspPermissionService, CspPermissionService>();
        services.AddTransient<ICspContentBuilder, CspContentBuilder>();
        services.AddTransient<IHeaderCompilationService, HeaderCompilationService>();
        services.AddTransient<ICspSettingsRepository, CspSettingsRepository>();
        services.AddTransient<ICspSettingsService, CspSettingsService>();
        services.AddTransient<ISecurityHeaderRepository, SecurityHeaderRepository>();
        services.AddTransient<ISecurityHeaderService, SecurityHeaderService>();
        services.AddTransient<ICspViolationReportRepository, CspViolationReportRepository>();
        services.AddTransient<IWhitelistRepository, WhitelistRepository>();
        services.AddTransient<IWhitelistService, WhitelistService>();
        services.AddTransient<ICacheWrapper, CacheWrapper>();
        services.AddTransient<IAuditRepository, AuditRepository>();
    }

    internal static void SetUpCspDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<CspDataContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly("Stott.Security.optimizely");
            });
        });

        services.AddScoped<ICspDataContext, CspDataContext>();
    }
}
