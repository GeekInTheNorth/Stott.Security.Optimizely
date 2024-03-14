namespace Stott.Security.Optimizely.Features.Configuration;

using System;
using System.Collections.Generic;

using EPiServer.ServiceLocation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.AllowList;
using Stott.Security.Optimizely.Features.Audit;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Cors.Provider;
using Stott.Security.Optimizely.Features.Cors.Repository;
using Stott.Security.Optimizely.Features.Cors.Service;
using Stott.Security.Optimizely.Features.Header;
using Stott.Security.Optimizely.Features.Middleware;
using Stott.Security.Optimizely.Features.Nonce;
using Stott.Security.Optimizely.Features.Permissions.List;
using Stott.Security.Optimizely.Features.Permissions.Repository;
using Stott.Security.Optimizely.Features.Permissions.Service;
using Stott.Security.Optimizely.Features.Reporting.Repository;
using Stott.Security.Optimizely.Features.Reporting.Service;
using Stott.Security.Optimizely.Features.Sandbox.Repository;
using Stott.Security.Optimizely.Features.Sandbox.Service;
using Stott.Security.Optimizely.Features.SecurityHeaders.Repository;
using Stott.Security.Optimizely.Features.SecurityHeaders.Service;
using Stott.Security.Optimizely.Features.Settings.Repository;
using Stott.Security.Optimizely.Features.Settings.Service;
using Stott.Security.Optimizely.Features.StaticFile;
using Stott.Security.Optimizely.Features.Tools;

public static class SecurityServiceExtensions
{
    /// <summary>
    /// Sets up configuration, dependencies, module access permissions and CORS.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setUpOptions">If left null, then the default will be to use the 'EPiServerDB' connection string.</param>
    /// <param name="authorizationOptions">If left null, then the default will be to require 'CmsAdmins', 'Administrator' and 'WebAdmins' roles.</param>
    /// <returns></returns>
    public static IServiceCollection AddStottSecurity(
        this IServiceCollection services,
        Action<SecuritySetupOptions>? setUpOptions = null,
        Action<AuthorizationOptions>? authorizationOptions = null)
    {
        var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

        // Handle null CSP Setup Options.
        var concreteOptions = new SecuritySetupOptions();
        if (setUpOptions != null)
        {
            setUpOptions(concreteOptions);
        }
        else
        {
            concreteOptions.ConnectionStringName = "EPiServerDB";
        }

        // Service Dependencies
        services.SetUpSecurityDependencies();

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
        services.SetUpSecurityDatabase(connectionString);

        // CORS
        services.AddTransient<ICorsPolicyProvider, CustomCorsPolicyProvider>();
        services.AddCors();

        return services;
    }

    [Obsolete("Please use .AddStottSecurity(...) instead. This method may be removed in version 3.")]
    public static IServiceCollection AddCspManager(
        this IServiceCollection services, 
        Action<SecuritySetupOptions>? cspSetupOptions = null, 
        Action<AuthorizationOptions>? authorizationOptions = null)
    {
        return services.AddStottSecurity(cspSetupOptions, authorizationOptions);
    }

    /// <summary>
    /// Sets up Stott Security middleware and adds CORS.
    /// </summary>
    /// <param name="builder"></param>
    public static void UseStottSecurity(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<SecurityHeaderMiddleware>();
        builder.UseCors(CspConstants.CorsPolicy);

        using var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetService<CspDataContext>();
        context?.Database.Migrate();
    }

    [Obsolete("Please use .UseStottSecurity(...) instead. This method may be removed in version 3.")]
    public static void UseCspManager(this IApplicationBuilder builder)
    {
        builder.UseStottSecurity();
    }

    internal static void SetUpSecurityDependencies(this IServiceCollection services)
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
        services.AddTransient<ICspViolationReportService, CspViolationReportService>();
        services.AddTransient<IAllowListRepository, AllowListRepository>();
        services.AddTransient<IAllowListService, AllowListService>();
        services.AddTransient<ICacheWrapper, CacheWrapper>();
        services.AddTransient<IAuditRepository, AuditRepository>();
        services.AddTransient<ICspSandboxRepository, CspSandboxRepository>();
        services.AddTransient<ICspSandboxService, CspSandboxService>();
        services.AddTransient<IStaticFileResolver, StaticFileResolver>();
        services.AddTransient<ICorsSettingsRepository, CorsSettingsRepository>();
        services.AddTransient<ICorsSettingsService, CorsSettingsService>();
        services.AddScoped<ICspReportUrlResolver, CspReportUrlResolver>();
        services.AddScoped<INonceProvider, DefaultNonceProvider>();
        services.AddScoped<IReportingEndpointValidator, ReportingEndpointValidator>();
        services.AddTransient<IMigrationService, MigrationService>();

        services.AddContentSecurityPolicyNonce(sp => sp.GetRequiredService<INonceProvider>().GetNonce());
    }

    internal static void SetUpSecurityDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<CspDataContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly("Stott.Security.Optimizely");
            });
        });

        services.AddScoped<ICspDataContext, CspDataContext>();
    }
}