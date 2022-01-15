using System.Linq;

using Stott.Optimizely.Csp.Features.Permissions.Repository;

namespace Stott.Optimizely.Csp.Features.Header
{
    public interface ISecurityHeaderService
    {
        string GetCspContent();
    }

    public class SecurityHeaderService : ISecurityHeaderService
    {
        private readonly ICspPermissionRepository _repository;

        private readonly ICspContentBuilder _headerBuilder;

        public SecurityHeaderService(
            ICspPermissionRepository repository, 
            ICspContentBuilder headerBuilder)
        {
            _repository = repository;
            _headerBuilder = headerBuilder;
        }

        public string GetCspContent()
        {
            var cspSources = _repository.Get();
            var cmsReqirements = _repository.GetCmsRequirements();
            var allSources = cspSources.Union(cmsReqirements).ToList();

            return _headerBuilder.WithSources(allSources).Build();
        }
    }
}
