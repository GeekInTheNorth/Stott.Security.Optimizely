using System;
using System.ComponentModel.DataAnnotations.Schema;

using Stott.Security.Core.Features.SecurityHeaders.Enums;

namespace Stott.Security.Core.Entities
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
