using Microsoft.Extensions.DependencyInjection;

using Stott.Optimizely.Csp.Features.Permissions.List;

namespace Stott.Optimizely.Csp.Features.Configuration
{
    public static class CspServiceExtensions
    {
        public static IServiceCollection AddCspManager(this IServiceCollection services)
        {
            services.AddTransient<ICspPermissionsViewModelBuilder, CspPermissionsViewModelBuilder>();

            return services;
        }
    }
}
