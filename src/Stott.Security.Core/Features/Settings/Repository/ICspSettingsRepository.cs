namespace Stott.Security.Core.Features.Settings.Repository;

using System.Threading.Tasks;

using Stott.Security.Core.Entities;

public interface ICspSettingsRepository
{
    Task<CspSettings> GetAsync();

    Task SaveAsync(bool isEnabled, bool isReportOnly);
}
