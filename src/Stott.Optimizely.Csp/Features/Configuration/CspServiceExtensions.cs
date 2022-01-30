using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Stott.Optimizely.Csp.Features.Header;
using Stott.Optimizely.Csp.Features.Permissions.List;
using Stott.Optimizely.Csp.Features.Permissions.Repository;
using Stott.Optimizely.Csp.Features.Settings.Repository;

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

            return services;
        }

        public static void UseCspManager(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<SecurityHeaderMiddleware>();
        }
    }
}
