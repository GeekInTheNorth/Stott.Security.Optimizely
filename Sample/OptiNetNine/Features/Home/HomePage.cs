namespace OptiNetNine.Features.Home
{
    using Common.Pages;

    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.DataAnnotations;
    using EPiServer.Shell.ObjectEditing;
    using EPiServer.Web;

    using Stott.Security.Optimizely.Features.Pages;

    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [ContentType(
        DisplayName = "Home Page",
        GUID = "060C7B3A-971D-4632-92C4-B493C2DA8D52",
        Description = "A page designed as a default landing page.",
        GroupName = SystemTabNames.Content)]
    public partial class HomePage : SitePageData, IContentSecurityPolicyPage
    {
        [Display(
            Name = "Hero Image",
            Description = "The image to render at the top of the page",
            GroupName = SystemTabNames.Content,
            Order = 10)]
        [UIHint(UIHint.Image)]
        public virtual ContentReference HeroImage { get; set; }

        [Display(
            Name = "Heading",
            Description = "The H1 to display",
            GroupName = SystemTabNames.Content,
            Order = 20)]
        public virtual string Heading { get; set; }

        [Display(
            Name = "Main Content Area",
            Description = "Renders blocks within the main content section of the home page.",
            GroupName = SystemTabNames.Content, 
            Order = 30)]
        public virtual ContentArea MainContentArea { get; set; }

        [Display(
            Name = "Content Security Policy Sources",
            GroupName = "Security",
            Order = 40)]
        [EditorDescriptor(EditorDescriptorType = typeof(CspSourceMappingEditorDescriptor))]
        public virtual IList<PageCspSourceMapping> ContentSecurityPolicySources { get; set; }
    }
}
