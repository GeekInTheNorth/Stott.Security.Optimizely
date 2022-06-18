namespace Stott.Security.Core.Features.Settings.Service;

using System.Threading.Tasks;

using Stott.Security.Core.Entities;

public interface ICspSettingsService
{
    Task<CspSettings> GetAsync();

    Task SaveAsync(bool isEnabled, bool isReportOnly);
}
