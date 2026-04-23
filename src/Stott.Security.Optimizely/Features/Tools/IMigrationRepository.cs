using System;
using System.Threading.Tasks;
using Stott.Security.Optimizely.Features.Tools.Models;

namespace Stott.Security.Optimizely.Features.Tools;

public interface IMigrationRepository
{
    Task SaveAsync(SettingsModel? settings, string? modifiedBy, Guid? siteId = null, string? hostName = null);
}