using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
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

            services.AddSingleton<ICspOptions>(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var options = configuration.GetSection("Csp").Get<CspOptions>() ?? new CspOptions();
                options.UseWhitelist = options.UseWhitelist && Uri.IsWellFormedUriString(options.WhitelistUrl, UriKind.Absolute);

                var connectionStringName = string.IsNullOrWhiteSpace(options.ConnectionStringName) ? "EPiServerDB" : options.ConnectionStringName;
                options.ConnectionString = configuration.GetConnectionString(connectionStringName);

                return options;
            });

            var cspOptions = services.BuildServiceProvider().GetService<ICspOptions>();
            services.AddDbContext<CspDataContext>(options =>
            {
                options.UseSqlServer(cspOptions.ConnectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("Stott.Optimizely.Csp");
                });
            });

            services.AddScoped<ICspDataContext, CspDataContext>();

            return services;
        }

        public static void UseCspManager(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<SecurityHeaderMiddleware>();

            using var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetService<CspDataContext>();
            context.Database.Migrate();
        }
    }
}
