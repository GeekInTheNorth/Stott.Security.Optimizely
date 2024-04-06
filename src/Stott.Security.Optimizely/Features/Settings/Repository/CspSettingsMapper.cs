using Stott.Security.Optimizely.Entities;

namespace Stott.Security.Optimizely.Features.Settings.Repository;

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
        entity.IsNonceEnabled = model.IsNonceEnabled;
        entity.IsStrictDynamicEnabled = model.IsStrictDynamicEnabled;
        entity.UseInternalReporting = model.UseInternalReporting;
        entity.UseExternalReporting = model.UseExternalReporting;
        entity.ExternalReportToUrl = model.ExternalReportToUrl;
        entity.ExternalReportUriUrl = model.ExternalReportUriUrl;
    }
}