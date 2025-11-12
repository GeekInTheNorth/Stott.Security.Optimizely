namespace OptimizelyTwelveTest.Features.Common
{
    using EPiServer.DataAbstraction;
    using EPiServer.DataAnnotations;

    using System.ComponentModel.DataAnnotations;

    [GroupDefinitions]
    public static class GroupNames
    {
        [Display(Order = 10)]
        public const string Content = SystemTabNames.Content;

        [Display(Order = 20)]
        public const string Teaser = "Teaser";

        [Display(Order = 30)]
        public const string SearchEngineOptimization = "SEO";

        [Display(Order = 40)]
        public const string Settings = SystemTabNames.Settings;
    }
}
