#nullable disable
namespace Stott.Security.Optimizely.Entities;

using System;
using System.ComponentModel.DataAnnotations.Schema;

using Stott.Security.Optimizely.Features.Audit;

[Table("tbl_CspSecurityHeaderSettings")]
public class SecurityHeaderSettings : IAuditableEntity
{
    public Guid Id { get; set; }

    public int XContentTypeOptions { get; set; }
    
    public int XssProtection { get; set; }

    public int ReferrerPolicy { get; set; }

    public int FrameOptions { get; set; }

    public int CrossOriginEmbedderPolicy { get; set; }

    public int CrossOriginOpenerPolicy { get; set; }

    public int CrossOriginResourcePolicy { get; set; }

    public bool IsStrictTransportSecurityEnabled { get; set; }

    public bool IsStrictTransportSecuritySubDomainsEnabled { get; set; }

    public int StrictTransportSecurityMaxAge { get; set; }  

    public bool ForceHttpRedirect { get; set; }

    public DateTime Modified { get; set; }

    public string ModifiedBy { get; set; }
}