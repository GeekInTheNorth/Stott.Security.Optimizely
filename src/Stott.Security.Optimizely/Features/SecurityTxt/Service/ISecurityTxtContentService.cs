using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stott.Security.Optimizely.Features.SecurityTxt.Service;

public interface ISecurityTxtContentService
{
    IList<SiteSecurityTxtViewModel> GetAll();

    SiteSecurityTxtViewModel? Get(Guid id);

    string? GetSecurityTxtContent(Guid siteId, string? host);

    Task SaveAsync(SaveSecurityTxtModel model, string? modifiedBy);

    Task DeleteAsync(Guid id, string? modifiedBy);

    bool DoesConflictExists(SaveSecurityTxtModel model);
}
