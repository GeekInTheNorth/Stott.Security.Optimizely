using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.VisualBuilder;

namespace OptimizelyTwelveTest.Features.Blocks.Sections;

[ContentType(DisplayName = "Standard Section", GUID = "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d", Description = "A standard section for campaign pages.")]
public class StandardSection : SectionData
{
    public override void SetDefaultValues(ContentType contentType)
    {
        base.SetDefaultValues(contentType);
        // Layout.Type should be "grid" for the grid/row/column tag helpers
        Layout = new Layout("grid");
        var row = new LayoutStructureNode("row");
        
        var left = new LayoutStructureNode("column");
        left.DisplaySettings["col"] = "col-md-9";
        
        var right = new LayoutStructureNode("column");
        right.DisplaySettings["col"] = "col-md-3";
        
        row.Nodes.Add(left);
        row.Nodes.Add(right);
        
        Layout.Nodes.Add(row);
    }
}
