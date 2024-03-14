using System.Threading.Tasks;

namespace Stott.Security.Optimizely.Features.Tools;

public interface IMigrationService
{
    Task<SettingsModel> Export();
}