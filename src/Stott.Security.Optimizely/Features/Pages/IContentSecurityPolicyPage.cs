namespace Stott.Security.Optimizely.Features.Pages;

using System.Collections.Generic;

public interface IContentSecurityPolicyPage
{
    IList<PageCspSourceMapping> ContentSecurityPolicySources { get; }
}