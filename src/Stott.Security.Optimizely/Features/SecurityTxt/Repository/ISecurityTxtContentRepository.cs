using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;

namespace Stott.Security.Optimizely.Features.SecurityTxt.Repository;

public interface ISecurityTxtContentRepository
{
    List<SecurityTxtEntity> GetAll();

    SecurityTxtEntity? Get(Guid id);

    Task SaveAsync(SaveSecurityTxtModel model, string modifiedBy);

    Task DeleteAsync(Guid id, string modifiedBy);
}