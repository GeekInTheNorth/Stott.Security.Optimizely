using System.Threading.Tasks;

namespace Stott.Security.Optimizely.Features.Tools;

public interface IMigrationService
{
    Task<SettingsModel> Export(string? appId = null, string? hostName = null);

    Task Import(SettingsModel? settings, string? modifiedBy, string? appId = null, string? hostName = null);
}