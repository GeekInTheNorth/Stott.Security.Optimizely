#nullable disable

using System;
using System.ComponentModel.DataAnnotations.Schema;

using Stott.Security.Optimizely.Features.Audit;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Entities;

[Table("tbl_stott_permissionpolicysettings")]
public class PermissionPolicySettings : IAuditableEntity, IPermissionPolicySettings
{
    public Guid Id { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime Modified { get; set; }

    public string ModifiedBy { get; set; }
}