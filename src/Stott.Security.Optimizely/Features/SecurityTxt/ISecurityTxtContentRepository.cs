using System;
using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.SecurityTxt;

public interface ISecurityTxtContentRepository
{
    List<SecurityTxtEntity> GetAll();

    List<SecurityTxtEntity> GetAllForSite(Guid siteId);

    SecurityTxtEntity? Get(Guid id);

    void Save(SaveSecurityTxtModel model);

    void Delete(Guid id);
}