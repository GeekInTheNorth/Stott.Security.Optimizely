namespace Stott.Security.Optimizely.Entities;

using System;
using System.ComponentModel.DataAnnotations.Schema;

using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;

[Table("tbl_CspSecurityHeaderSettings")]
public class SecurityHeaderSettings
{
    public Guid Id { get; set; }

    public XContentTypeOptions XContentTypeOptions { get; set; }
    
    public XssProtection XssProtection { get; set; }

    public ReferrerPolicy ReferrerPolicy { get; set; }

    public XFrameOptions FrameOptions { get; set; }

    public CrossOriginEmbedderPolicy CrossOriginEmbedderPolicy { get; set; }

    public CrossOriginOpenerPolicy CrossOriginOpenerPolicy { get; set; }

    public CrossOriginResourcePolicy CrossOriginResourcePolicy { get; set; }

    public bool IsStrictTransportSecurityEnabled { get; set; }

    public bool IsStrictTransportSecuritySubDomainsEnabled { get; set; }

    public int StrictTransportSecurityMaxAge { get; set; }  

    public bool ForceHttpRedirect { get; set; }
}
