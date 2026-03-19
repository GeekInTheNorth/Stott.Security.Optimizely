#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Features.Audit;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Entities;

[Table("tbl_stott_permissionpolicysettings")]
[Index(nameof(AppId), nameof(HostName), IsUnique = false, Name = "idx_permissionpolicysettings_AppId_HostName")]
public class PermissionPolicySettings : IAuditableEntity, IPermissionPolicySettings
{
    public Guid Id { get; set; }

    public bool IsEnabled { get; set; }

    [MaxLength(200)]
    public string AppId { get; set; }

    [MaxLength(200)]
    public string HostName { get; set; }

    public DateTime Modified { get; set; }

    public string ModifiedBy { get; set; }
}