using System;
using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.SecurityTxt.Service;

public interface ISecurityTxtContentService
{
    IList<SiteSecurityTxtViewModel> GetAll();

    SiteSecurityTxtViewModel? Get(Guid id);

    string? GetSecurityTxtContent(Guid siteId, string? host);

    string? GetDefaultSecurityTxtContent();

    void Save(SaveSecurityTxtModel model, string? modifiedBy);

    void Delete(Guid id, string? modifiedBy);

    bool DoesConflictExists(SaveSecurityTxtModel model);
}
