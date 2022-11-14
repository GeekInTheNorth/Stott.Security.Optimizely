namespace Stott.Security.Optimizely.Entities;

using System;
using System.ComponentModel.DataAnnotations.Schema;

using Stott.Security.Optimizely.Features.Audit;

[Table("tbl_CspSandbox")]
public class CspSandbox : IAuditableEntity
{
    public Guid Id { get; set; }

    public bool IsSandboxEnabled { get; set; }

    public bool IsAllowDownloadsEnabled { get; set; }

    public bool IsAllowDownloadsWithoutGestureEnabled { get; set; }

    public bool IsAllowFormsEnabled { get; set; }

    public bool IsAllowModalsEnabled { get; set; }

    public bool IsAllowOrientationLockEnabled { get; set; }

    public bool IsAllowPointerLockEnabled { get; set; }

    public bool IsAllowPopupsEnabled { get; set; }

    public bool IsAllowPopupsToEscapeTheSandboxEnabled { get; set; }

    public bool IsAllowPresentationEnabled { get; set; }

    public bool IsAllowSameOriginEnabled { get; set; }

    public bool IsAllowScriptsEnabled { get; set; }

    public bool IsAllowStorageAccessByUserEnabled { get; set; }

    public bool IsAllowTopNavigationEnabled { get; set; }

    public bool IsAllowTopNavigationByUserEnabled { get; set; }

    public bool IsAllowTopNavigationToCustomProtocolEnabled { get; set; }

    public DateTime Modified { get; set; }

    public string ModifiedBy { get; set; }
}