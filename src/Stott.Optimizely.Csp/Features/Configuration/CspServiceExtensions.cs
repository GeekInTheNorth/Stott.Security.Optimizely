using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Features.Header;
using Stott.Optimizely.Csp.Features.Permissions.List;
using Stott.Optimizely.Csp.Features.Permissions.Repository;
using Stott.Optimizely.Csp.Features.Reporting.Repository;
using Stott.Optimizely.Csp.Features.SecurityHeaders.Repository;
using Stott.Optimizely.Csp.Features.Settings.Repository;
using Stott.Optimizely.Csp.Features.Whitelist;

namespace Stott.Optimizely.Csp.Features.Configuration
{
    public static class CspServiceExtensions
    {
        public static IServiceCollection AddCspManager(this IServiceCollection services)
        {
            services.AddTransient<ICspPermissionsListModelBuilder, CspPermissionsListModelBuilder>();
            services.AddTransient<ICspPermissionRepository, CspPermissionRepository>();
            services.AddTransient<ICspContentBuilder, CspContentBuilder>();
            services.AddTransient<ISecurityHeaderService, SecurityHeaderService>();
            services.AddTransient<ICspSettingsRepository, CspSettingsRepository>();
            services.AddTransient<ISecurityHeaderRepository, SecurityHeaderRepository>();
            services.AddTransient<ICspViolationReportRepository, CspViolationReportRepository>();
            services.AddTransient<IWhitelistRepository, WhitelistRepository>();
            services.AddTransient<IWhitelistService, WhitelistService>();

            services.AddSingleton<ICspWhitelistOptions>(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var whiteListOptions = configuration.GetSection("Csp").Get<CspWhitelistOptions>() ?? new CspWhitelistOptions();
                whiteListOptions.UseWhitelist = whiteListOptions.UseWhitelist && Uri.IsWellFormedUriString(whiteListOptions.WhitelistUrl, UriKind.Absolute);
                whiteListOptions.ConnectionString = configuration.GetConnectionString(whiteListOptions.ConnectionStringName);

                return whiteListOptions;
            });

            services.AddScoped<CspDataContext>();

            return services;
        }

        public static void UseCspManager(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<SecurityHeaderMiddleware>();
        }
    }
}
