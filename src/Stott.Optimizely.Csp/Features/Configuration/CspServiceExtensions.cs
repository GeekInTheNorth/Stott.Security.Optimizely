using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Stott.Optimizely.Csp.Features.Header;
using Stott.Optimizely.Csp.Features.Permissions.List;
using Stott.Optimizely.Csp.Features.Permissions.Repository;

namespace Stott.Optimizely.Csp.Features.Configuration
{
    public static class CspServiceExtensions
    {
        public static IServiceCollection AddCspManager(this IServiceCollection services)
        {
            services.AddTransient<ICspPermissionsViewModelBuilder, CspPermissionsViewModelBuilder>();
            services.AddTransient<ICspPermissionRepository, CspPermissionRepository>();
            services.AddTransient<ICspContentBuilder, CspContentBuilder>();
            services.AddTransient<ISecurityHeaderService, SecurityHeaderService>();

            return services;
        }

        public static void UseCspManager(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<SecurityHeaderMiddleware>();
        }
    }
}
