namespace Stott.Security.Optimizely.Features.Guides.Service;

using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Guides.Models;

public interface IGuideService
{
    /// <summary>
    /// Retrieves the published Stott Security guides, newest first.
    /// Returns an empty list if the remote feed cannot be reached.
    /// </summary>
    Task<GuideCollection> GetGuidesAsync();
}
