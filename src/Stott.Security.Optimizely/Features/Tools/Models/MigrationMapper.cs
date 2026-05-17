using System;
using System.Collections.Generic;
using System.Linq;
using Stott.Security.Optimizely.Extensions;
using Stott.Security.Optimizely.Features.Csp.Sandbox;
using Stott.Security.Optimizely.Features.PermissionPolicy;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Features.Tools.Models
{
    internal static class MigrationMapper
    {
        public static CspSourceMigrationModel ConvertToModel(Entities.CspSource entity)
        {
            return new CspSourceMigrationModel
            {
                Source = entity.Source,
                Directives = entity.Directives?.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? new List<string>(0)
            };
        }

        public static Entities.CspSource ConvertToEntity(CspSourceMigrationModel model, Guid? siteId, string? hostName, DateTime modified, string modifiedBy)
        {
            return new Entities.CspSource
            {
                Source = model.Source,
                SiteId = siteId,
                HostName = hostName,
                Directives = model.Directives != null ? string.Join(",", model.Directives) : null,
                Modified = modified,
                ModifiedBy = modifiedBy
            };
        }

        public static void MapToEntity(Entities.CspSource entity, CspSourceMigrationModel model, DateTime modified, string modifiedBy)
        {
            entity.Source = model.Source;
            entity.Directives = model.Directives != null ? string.Join(",", model.Directives) : null;
            entity.Modified = modified;
            entity.ModifiedBy = modifiedBy;
        }

        public static CspSettingsMigrationModel ConvertToModel(Entities.CspSettings? settings, IList<Entities.CspSource>? sources, SandboxModel? sandbox)
        {
            return new CspSettingsMigrationModel
            {
                IsEnabled = settings?.IsEnabled ?? false,
                IsReportOnly = settings?.IsReportOnly ?? false,
                IsAllowListEnabled = settings?.IsAllowListEnabled ?? false,
                AllowListUrl = settings?.AllowListUrl,
                IsUpgradeInsecureRequestsEnabled = settings?.IsUpgradeInsecureRequestsEnabled ?? false,
                UseInternalReporting = settings?.UseInternalReporting ?? false,
                UseExternalReporting = settings?.UseExternalReporting ?? false,
                ExternalReportToUrl = settings?.ExternalReportToUrl,
                Sources = sources?.Select(ConvertToModel).ToList() ?? new List<CspSourceMigrationModel>(0),
                Sandbox = ConvertToModel(sandbox)
            };
        }

        public static Entities.CspSettings ConvertToEntity(CspSettingsMigrationModel model, Guid? siteId, string? hostName, DateTime modified, string modifiedBy)
        {
            return new Entities.CspSettings
            {
                IsEnabled = model.IsEnabled,

                // When importing we should always match report mode to the enabled state in case of CSP issues.
                // This will allow the user to test the CSP before enforcing it.
                IsReportOnly = model.IsEnabled,
                IsAllowListEnabled = model.IsAllowListEnabled,
                AllowListUrl = model.AllowListUrl,
                IsUpgradeInsecureRequestsEnabled = model.IsUpgradeInsecureRequestsEnabled,
                UseInternalReporting = model.UseInternalReporting,
                UseExternalReporting = model.UseExternalReporting,
                ExternalReportToUrl = model.ExternalReportToUrl,
                SiteId = siteId,
                HostName = hostName,
                Modified = modified,
                ModifiedBy = modifiedBy
            };
        }

        public static void MapToEntity(Entities.CspSettings entity, CspSettingsMigrationModel model, DateTime modified, string modifiedBy)
        {
            entity.IsEnabled = model.IsEnabled;

            // When importing we should always match report mode to the enabled state in case of CSP issues.
            // This will allow the user to test the CSP before enforcing it.
            entity.IsReportOnly = model.IsEnabled; 
            entity.IsAllowListEnabled = model.IsAllowListEnabled;
            entity.AllowListUrl = model.AllowListUrl;
            entity.IsUpgradeInsecureRequestsEnabled = model.IsUpgradeInsecureRequestsEnabled;
            entity.UseInternalReporting = model.UseInternalReporting;
            entity.UseExternalReporting = model.UseExternalReporting;
            entity.ExternalReportToUrl = model.ExternalReportToUrl;
            entity.Modified = modified;
            entity.ModifiedBy = modifiedBy;
        }

        public static CspSandboxMigrationModel ConvertToModel(SandboxModel? sandbox)
        {
            return new CspSandboxMigrationModel
            {
                IsSandboxEnabled = sandbox?.IsSandboxEnabled ?? false,
                IsAllowDownloadsEnabled = sandbox?.IsAllowDownloadsEnabled ?? false,
                IsAllowDownloadsWithoutGestureEnabled = sandbox?.IsAllowDownloadsWithoutGestureEnabled ?? false,
                IsAllowFormsEnabled = sandbox?.IsAllowFormsEnabled ?? false,
                IsAllowModalsEnabled = sandbox?.IsAllowModalsEnabled ?? false,
                IsAllowOrientationLockEnabled = sandbox?.IsAllowOrientationLockEnabled ?? false,
                IsAllowPointerLockEnabled = sandbox?.IsAllowPointerLockEnabled ?? false,
                IsAllowPopupsEnabled = sandbox?.IsAllowPopupsEnabled ?? false,
                IsAllowPopupsToEscapeTheSandboxEnabled = sandbox?.IsAllowPopupsToEscapeTheSandboxEnabled ?? false,
                IsAllowPresentationEnabled = sandbox?.IsAllowPresentationEnabled ?? false,
                IsAllowSameOriginEnabled = sandbox?.IsAllowSameOriginEnabled ?? false,
                IsAllowScriptsEnabled = sandbox?.IsAllowScriptsEnabled ?? false,
                IsAllowStorageAccessByUserEnabled = sandbox?.IsAllowStorageAccessByUserEnabled ?? false,
                IsAllowTopNavigationEnabled = sandbox?.IsAllowTopNavigationEnabled ?? false,
                IsAllowTopNavigationByUserEnabled = sandbox?.IsAllowTopNavigationByUserEnabled ?? false,
                IsAllowTopNavigationToCustomProtocolEnabled = sandbox?.IsAllowTopNavigationToCustomProtocolEnabled ?? false
            };
        }

        public static Entities.CspSandbox ConvertToEntity(CspSandboxMigrationModel model, Guid? siteId, string? hostName, DateTime modified, string modifiedBy)
        {
            return new Entities.CspSandbox
            {
                IsSandboxEnabled = model.IsSandboxEnabled,
                IsAllowDownloadsEnabled = model.IsAllowDownloadsEnabled,
                IsAllowDownloadsWithoutGestureEnabled = model.IsAllowDownloadsWithoutGestureEnabled,
                IsAllowFormsEnabled = model.IsAllowFormsEnabled,
                IsAllowModalsEnabled = model.IsAllowModalsEnabled,
                IsAllowOrientationLockEnabled = model.IsAllowOrientationLockEnabled,
                IsAllowPointerLockEnabled = model.IsAllowPointerLockEnabled,
                IsAllowPopupsEnabled = model.IsAllowPopupsEnabled,
                IsAllowPopupsToEscapeTheSandboxEnabled = model.IsAllowPopupsToEscapeTheSandboxEnabled,
                IsAllowPresentationEnabled = model.IsAllowPresentationEnabled,
                IsAllowSameOriginEnabled = model.IsAllowSameOriginEnabled,
                IsAllowScriptsEnabled = model.IsAllowScriptsEnabled,
                IsAllowStorageAccessByUserEnabled = model.IsAllowStorageAccessByUserEnabled,
                IsAllowTopNavigationEnabled = model.IsAllowTopNavigationEnabled,
                IsAllowTopNavigationByUserEnabled = model.IsAllowTopNavigationByUserEnabled,
                IsAllowTopNavigationToCustomProtocolEnabled = model.IsAllowTopNavigationToCustomProtocolEnabled,
                SiteId = siteId,
                HostName = hostName,
                Modified = modified,
                ModifiedBy = modifiedBy
            };
        }

        public static void MapToEntity(Entities.CspSandbox entity, CspSandboxMigrationModel model, DateTime modified, string modifiedBy)
        {
            entity.IsSandboxEnabled = model.IsSandboxEnabled;
            entity.IsAllowDownloadsEnabled = model.IsAllowDownloadsEnabled;
            entity.IsAllowDownloadsWithoutGestureEnabled = model.IsAllowDownloadsWithoutGestureEnabled;
            entity.IsAllowFormsEnabled = model.IsAllowFormsEnabled;
            entity.IsAllowModalsEnabled = model.IsAllowModalsEnabled;
            entity.IsAllowOrientationLockEnabled = model.IsAllowOrientationLockEnabled;
            entity.IsAllowPointerLockEnabled = model.IsAllowPointerLockEnabled;
            entity.IsAllowPopupsEnabled = model.IsAllowPopupsEnabled;
            entity.IsAllowPopupsToEscapeTheSandboxEnabled = model.IsAllowPopupsToEscapeTheSandboxEnabled;
            entity.IsAllowPresentationEnabled = model.IsAllowPresentationEnabled;
            entity.IsAllowSameOriginEnabled = model.IsAllowSameOriginEnabled;
            entity.IsAllowScriptsEnabled = model.IsAllowScriptsEnabled;
            entity.IsAllowStorageAccessByUserEnabled = model.IsAllowStorageAccessByUserEnabled;
            entity.IsAllowTopNavigationEnabled = model.IsAllowTopNavigationEnabled;
            entity.IsAllowTopNavigationByUserEnabled = model.IsAllowTopNavigationByUserEnabled;
            entity.IsAllowTopNavigationToCustomProtocolEnabled = model.IsAllowTopNavigationToCustomProtocolEnabled;
            entity.Modified = modified;
            entity.ModifiedBy = modifiedBy;
        }

        public static PermissionPolicyMigrationModel ConvertToModel(Entities.PermissionPolicySettings? settings, List<Entities.PermissionPolicy>? policies)
        {
            return new PermissionPolicyMigrationModel
            {
                IsEnabled = settings?.IsEnabled ?? false,
                Directives = policies?.Select(ConvertToModel).ToList() ?? new List<PermissionPolicyDirectiveMigrationModel>()
            };
        }

        public static Entities.PermissionPolicySettings ConvertToEntity(PermissionPolicyMigrationModel model, Guid? siteId, string? hostName, DateTime modified, string modifiedBy)
        {
            return new Entities.PermissionPolicySettings
            {
                IsEnabled = model.IsEnabled,
                SiteId = siteId,
                HostName = hostName,
                Modified = modified,
                ModifiedBy = modifiedBy
            };
        }

        public static void MapToEntity(Entities.PermissionPolicySettings entity, PermissionPolicyMigrationModel model, DateTime modified, string modifiedBy)
        {
            entity.IsEnabled = model.IsEnabled;
            entity.Modified = modified;
            entity.ModifiedBy = modifiedBy;
        }

        public static PermissionPolicyDirectiveMigrationModel ConvertToModel(Entities.PermissionPolicy entity)
        {
            return new PermissionPolicyDirectiveMigrationModel
            {
                Name = entity.Directive,
                EnabledState = entity.EnabledState.ToEnum(PermissionPolicyEnabledState.Disabled),
                Sources = entity.Origins.SplitByComma()
                             .Select(x => new PermissionPolicyUrl { Id = Guid.NewGuid(), Url = x })
                             .ToList()
            };
        }

        public static Entities.PermissionPolicy ConvertToEntity(PermissionPolicyDirectiveMigrationModel model, Guid? siteId, string? hostName, DateTime modified, string modifiedBy)
        {
            return new Entities.PermissionPolicy
            {
                Directive = model.Name,
                EnabledState = model.EnabledState.ToString(),
                Origins = string.Join(',', model.Sources?.Select(x => x.Url) ?? Enumerable.Empty<string>()),
                SiteId = siteId,
                HostName = hostName,
                Modified = modified,
                ModifiedBy = modifiedBy
            };
        }

        public static void MapToEntity(Entities.PermissionPolicy entity, PermissionPolicyDirectiveMigrationModel model, DateTime modified, string modifiedBy)
        {
            entity.Directive = model.Name;
            entity.EnabledState = model.EnabledState.ToString();
            entity.Origins = string.Join(',', model.Sources?.Select(x => x.Url) ?? Enumerable.Empty<string>());
            entity.Modified = modified;
            entity.ModifiedBy = modifiedBy;
        }

        public static CustomHeaderMigrationModel ConvertToModel(Entities.CustomHeader entity)
        {
            return new CustomHeaderMigrationModel
            {
                HeaderName = entity.HeaderName,
                Behavior = entity.Behavior,
                HeaderValue = entity.HeaderValue
            };
        }

        public static Entities.CustomHeader ConvertToEntity(CustomHeaderMigrationModel model, Guid? siteId, string? hostName, DateTime modified, string modifiedBy)
        {
            return new Entities.CustomHeader
            {
                HeaderName = model.HeaderName,
                Behavior = model.Behavior,
                HeaderValue = model.HeaderValue,
                SiteId = siteId,
                HostName = hostName,
                Modified = modified,
                ModifiedBy = modifiedBy
            };
        }

        public static void MapToEntity(Entities.CustomHeader entity, CustomHeaderMigrationModel model, DateTime modified, string modifiedBy)
        {
            entity.HeaderName = model.HeaderName;
            entity.Behavior = model.Behavior;
            entity.HeaderValue = model.HeaderValue;
            entity.Modified = modified;
            entity.ModifiedBy = modifiedBy;
        }
    }
}
