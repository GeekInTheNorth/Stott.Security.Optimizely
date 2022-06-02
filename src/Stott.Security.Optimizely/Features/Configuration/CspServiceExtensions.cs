using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                    sqlOptions.MigrationsAssembly("Stott.Security.Core");
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
