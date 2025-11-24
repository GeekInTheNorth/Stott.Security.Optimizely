using Stott.Security.Optimizely.Entities;

namespace Stott.Security.Optimizely.Features.Csp.Settings.Repository;

internal static class CspSettingsMapper
{
    internal static void ToEntity(ICspSettings? model, CspSettings entity)
    {
        if (model is null)
        {
            return;
        }

        entity.IsEnabled = model.IsEnabled;
        entity.IsReportOnly = model.IsReportOnly;
        entity.IsAllowListEnabled = model.IsAllowListEnabled;
        entity.AllowListUrl = model.AllowListUrl;
        entity.IsUpgradeInsecureRequestsEnabled = model.IsUpgradeInsecureRequestsEnabled;
        entity.IsNonceEnabled = false;
        entity.IsStrictDynamicEnabled = false;
        entity.UseInternalReporting = model.UseInternalReporting;
        entity.UseExternalReporting = model.UseExternalReporting;
        entity.ExternalReportToUrl = model.ExternalReportToUrl;
        entity.ExternalReportUriUrl = null; // Report-Uri is deprecated, but data may still exist in a database
    }
}