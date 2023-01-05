namespace Stott.Security.Optimizely.Features.Pages;

using System.Collections.Generic;
using System.Linq;

using EPiServer.Shell.ObjectEditing;

using Stott.Security.Optimizely.Common;

public sealed class CspDirectiveSelectionFactory : ISelectionFactory
{
    public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
    {
        return CspConstants.AllDirectives.Select(x => new SelectItem { Text = x, Value = x }).ToList();
    }
}