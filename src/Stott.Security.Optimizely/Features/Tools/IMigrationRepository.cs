using System.Threading.Tasks;

namespace Stott.Security.Optimizely.Features.Tools;

public interface IMigrationRepository
{
    Task SaveAsync(SettingsModel? settings, string? modifiedBy);
}