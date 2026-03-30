#nullable disable
namespace Stott.Security.Optimizely.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

[Table("tbl_StottV7_CspViolationSummary")]
[Index(nameof(BlockedUri), nameof(ViolatedDirective), nameof(AppId), nameof(HostName), IsUnique = false, Name = "idx_StottV7_CspViolationSummary_LookUp")]
public class CspViolationSummary
{
    public Guid Id { get; set; }

    [MaxLength(1000)]
    public string BlockedUri { get; set; }

    [MaxLength(100)]
    public string ViolatedDirective { get; set; }

    [MaxLength(200)]
    public string AppId { get; set; }

    [MaxLength(200)]
    public string HostName { get; set; }

    public DateTime LastReported { get; set; }

    public int Instances { get; set; }
}
