using System;
using System.ComponentModel.DataAnnotations.Schema;

using Stott.Optimizely.Csp.Features.SecurityHeaders.Enums;

namespace Stott.Optimizely.Csp.Entities
{
    [Table("tbl_CspSecurityHeaderSettings")]
    public class SecurityHeaderSettings
    {
        public Guid Id { get; set; }

        public bool IsXContentTypeOptionsEnabled { get; set; }

        public bool IsXXssProtectionEnabled { get; set; }

        public ReferrerPolicy ReferrerPolicy { get; set; }

        public XFrameOptions FrameOptions { get; set; }
    }
}
