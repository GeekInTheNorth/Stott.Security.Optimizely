using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stott.Security.Optimizely.Features.SecurityTxt.Models;

namespace Stott.Security.Optimizely.Features.SecurityTxt.Service;

public interface ISecurityTxtContentService
{
    Task<IList<SiteSecurityTxtViewModel>> GetAllAsync();

    Task<SiteSecurityTxtViewModel?> GetAsync(Guid id);

    string? GetSecurityTxtContent(string? appId, string? host);

    Task SaveAsync(SaveSecurityTxtModel model, string? modifiedBy);

    Task DeleteAsync(Guid id, string? modifiedBy);

    bool DoesConflictExists(SaveSecurityTxtModel model);
}
