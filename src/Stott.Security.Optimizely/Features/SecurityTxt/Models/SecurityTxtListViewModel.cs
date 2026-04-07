using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.SecurityTxt.Models;

public sealed class SecurityTxtListViewModel
{
    public IList<SiteSecurityTxtViewModel>? List { get; set; }
}
