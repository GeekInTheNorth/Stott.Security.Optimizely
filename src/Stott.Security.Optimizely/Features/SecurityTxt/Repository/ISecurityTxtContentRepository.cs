using System;
using System.Collections.Generic;
using Stott.Security.Optimizely.Entities;

namespace Stott.Security.Optimizely.Features.SecurityTxt.Repository;

public interface ISecurityTxtContentRepository
{
    List<SecurityTxtEntity> GetAll();

    List<SecurityTxtEntity> GetAllForSite(Guid siteId);

    SecurityTxtEntity? Get(Guid id);

    void Save(SaveSecurityTxtModel model, string modifiedBy);

    void Delete(Guid id, string modifiedBy);
}