namespace Stott.Security.Core.Features.SecurityHeaders
{
    public class SecurityHeaderModel
    {
        public bool IsXctoEnabled { get; set; }

        public bool IsXxpEnabled { get; set; }

        public string XFrameOptions { get; set; }

        public string ReferrerPolicy { get; set; }
    }
}
