namespace Stott.Security.Optimizely.Features.Route;

/// <summary>
/// Provides information about the current request route for security header processing.
/// </summary>
public interface ISecurityRouteHelper
{
    /// <summary>
    /// Get the security route data for the current request.
    /// </summary>
    /// <returns></returns>
    SecurityRouteData GetRouteData();
}