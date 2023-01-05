namespace Stott.Security.Optimizely.Test;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;

using Stott.Security.Optimizely.Features.Pages;

public class TestPageData : PageData, IContentSecurityPolicyPage
{
    [Display(
        Name = "Content Security Policy Sources",
        GroupName = "Security",
        Order = 40)]
    [EditorDescriptor(EditorDescriptorType = typeof(CspSourceMappingEditorDescriptor))]
    public virtual IList<PageCspSourceMapping> ContentSecurityPolicySources { get; set; }
}