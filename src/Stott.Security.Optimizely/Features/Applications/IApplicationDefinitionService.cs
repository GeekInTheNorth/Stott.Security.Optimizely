using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stott.Security.Optimizely.Features.Applications;

public interface IApplicationDefinitionService 
{
    Task<IEnumerable<ApplicationViewModel>> GetAllApplicationsAsync();

    Task<ApplicationViewModel?> GetApplicationByIdAsync(string? appId);
}
