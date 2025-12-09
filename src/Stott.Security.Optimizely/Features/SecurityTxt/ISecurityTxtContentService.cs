using System;
using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.SecurityTxt;

public interface ISecurityTxtContentService
{
    IList<SiteSecurityTxtViewModel> GetAll();

    SiteSecurityTxtViewModel Get(Guid id);

    SiteSecurityTxtViewModel GetDefault(Guid siteId);

    string? GetSecurityTxtContent(Guid siteId, string? host);

    string? GetDefaultSecurityTxtContent();

    void Save(SaveSecurityTxtModel model);

    void Delete(Guid id);

    bool DoesConflictExists(SaveSecurityTxtModel model);
}
