using System;
using System.Threading.Tasks;
using Stott.Security.Optimizely.Features.Tools.Models;

namespace Stott.Security.Optimizely.Features.Tools;

public interface IMigrationService
{
    Task<SettingsModel> Export(Guid? siteId = null, string? hostName = null);

    Task Import(SettingsModel? settings, string? modifiedBy, Guid? siteId = null, string? hostName = null);
}