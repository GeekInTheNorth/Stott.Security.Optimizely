namespace OptimizelyTwelveTest.Features.Common.Media
{
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.DataAnnotations;
    using EPiServer.Framework.DataAnnotations;

    using System.ComponentModel.DataAnnotations;

    [ContentType(DisplayName = "Image Content", GUID = "24c0b379-5101-4730-b660-255fee633316", Description = "Represents an image in PNG, JPG or GIF format.")]
    [MediaDescriptor(ExtensionString = "png,jpg,jpeg,gif")]
    public class ImageContent : ImageData
    {
        [CultureSpecific]
        [Display(
            Name = "Title",
            Description = "The title to render for the image.",
            GroupName = SystemTabNames.Content,
            Order = 10)]
        public virtual string Title { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Alt Text",
            Description = "The Alt Text for the image.",
            GroupName = SystemTabNames.Content,
            Order = 20)]
        public virtual string AltText { get; set; }
    }
}