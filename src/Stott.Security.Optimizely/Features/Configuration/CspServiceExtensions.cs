using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Stott.Security.Core.Common;
using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.Header;
using Stott.Security.Core.Features.Logging;
using Stott.Security.Core.Features.Permissions.List;
using Stott.Security.Core.Features.Permissions.Repository;
using Stott.Security.Core.Features.Reporting.Repository;
using Stott.Security.Core.Features.SecurityHeaders.Repository;
using Stott.Security.Core.Features.Settings.Repository;
using Stott.Security.Core.Features.Whitelist;
using Stott.Security.Optimizely.Features.Logging;
using Stott.Security.Optimizely.Features.Middleware;

namespace Stott.Security.Optimizely.Features.Configuration
{
    public static class CspServiceExtensions
    {
        public static IServiceCollection AddCspManager(this IServiceCollection services)
        {
            // Service Dependencies
            services.SetUpCspDependencies();

            // Whitelist Options
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            var options = configuration.GetSection("Csp").Get<CspOptions>() ?? new CspOptions();
            services.SetUpCspWhitelistOptions(options.UseWhitelist, options.WhitelistUrl);

            // Authorization
            var allowedRoles = options.AllowedRoles ?? "CmsAdmins,Administrator";
            var allowedRolesCollection = allowedRoles.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            services.SetUpCspAuthorization(allowedRolesCollection);

            // Database
            var connectionStringName = string.IsNullOrWhiteSpace(options.ConnectionStringName) ? "EPiServerDB" : options.ConnectionStringName;
            var connectionString = configuration.GetConnectionString(connectionStringName);
            services.SetUpCspDatabase(connectionString);

            return services;
        }

        public static IServiceCollection AddCspManager(this IServiceCollection services, Action<CspSetupOptions> cspSetupOptions)
        {
            var concreteOptions = new CspSetupOptions();
            cspSetupOptions(concreteOptions);

            // Service Dependencies
            services.SetUpCspDependencies();

            // Whitelist Options
            services.SetUpCspWhitelistOptions(concreteOptions.UseWhitelist, concreteOptions.WhitelistUrl);

            // Authorization
            services.SetUpCspAuthorization(concreteOptions.AllowedRoles);

            // Database
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            var connectionString = configuration.GetConnectionString(concreteOptions.ConnectionStringName);
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
            services.AddTransient<ILoggingProviderFactory, LoggingProviderFactory>();
            services.AddTransient<ICspPermissionsListModelBuilder, CspPermissionsListModelBuilder>();
            services.AddTransient<ICspPermissionRepository, CspPermissionRepository>();
            services.AddTransient<ICspContentBuilder, CspContentBuilder>();
            services.AddTransient<ISecurityHeaderService, SecurityHeaderService>();
            services.AddTransient<ICspSettingsRepository, CspSettingsRepository>();
            services.AddTransient<ISecurityHeaderRepository, SecurityHeaderRepository>();
            services.AddTransient<ICspViolationReportRepository, CspViolationReportRepository>();
            services.AddTransient<IWhitelistRepository, WhitelistRepository>();
            services.AddTransient<IWhitelistService, WhitelistService>();
        }

        internal static void SetUpCspDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<CspDataContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("Stott.Security.Core");
                });
            });

            services.AddScoped<ICspDataContext, CspDataContext>();
        }

        internal static void SetUpCspWhitelistOptions(this IServiceCollection services, bool useWhitelist, string whitelistUrl)
        {
            services.AddSingleton<ICspWhitelistOptions>(serviceProvider =>
            {
                return new CspWhiteListOptions
                {
                    UseWhitelist = useWhitelist && Uri.IsWellFormedUriString(whitelistUrl, UriKind.Absolute),
                    WhitelistUrl = whitelistUrl
                };
            });
        }

        internal static void SetUpCspAuthorization(this IServiceCollection services, IReadOnlyCollection<string> allowedRoles)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(CspConstants.AuthorizationPolicy, policy =>
                {
                    policy.RequireRole(allowedRoles);
                });
            });
        }
    }
}
