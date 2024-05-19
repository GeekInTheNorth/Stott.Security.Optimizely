using System.Collections.Generic;

namespace Stott.Security.Optimizely.Widget.HeaderPreview;

public sealed class HeaderPreviewWidgetViewModel
{
    public string? PageName { get; internal set; }

    public bool CanExtendTheContentSecurityPolicy { get; internal set; }

    public bool ExtendsTheContentSecurityPolicy { get; internal set; }

    public List<KeyValuePair<string, string>>? SecurityHeaders { get; internal set; }

    public string? Version { get; internal set; }

    public string? JavaScriptPath { get; internal set; }

    public string? CssPath { get; internal set; }
}