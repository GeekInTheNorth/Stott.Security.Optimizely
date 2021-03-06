using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Stott.Security.Core.Common;
using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.Caching;
using Stott.Security.Core.Features.Header;
using Stott.Security.Core.Features.Logging;
using Stott.Security.Core.Features.Permissions.List;
using Stott.Security.Core.Features.Permissions.Repository;
using Stott.Security.Core.Features.Permissions.Service;
using Stott.Security.Core.Features.Reporting.Repository;
using Stott.Security.Core.Features.SecurityHeaders.Repository;
using Stott.Security.Core.Features.SecurityHeaders.Service;
using Stott.Security.Core.Features.Settings.Repository;
using Stott.Security.Core.Features.Settings.Service;
using Stott.Security.Core.Features.Whitelist;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Logging;
using Stott.Security.Optimizely.Features.Middleware;

namespace Stott.Security.Optimizely.Features.Configuration
{
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

            // Whitelist Options
            services.SetUpCspWhitelistOptions(concreteOptions.UseWhitelist, concreteOptions.WhitelistUrl);

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
            services.AddTransient<ILoggingProviderFactory, LoggingProviderFactory>();
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
    }
}
