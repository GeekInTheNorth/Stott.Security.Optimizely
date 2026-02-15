namespace Stott.Security.Optimizely.Features.Audit;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Audit.Models;

public interface IAuditRepository
{
    Task<IEnumerable<AuditEntryModel>> GetAsync(
        DateTime dateFrom,
        DateTime dateTo,
        string? author,
        string? recordType,
        string? operationType,
        int from,
        int take,
        string? searchTerm);

    Task<IEnumerable<string>> GetUsersAsync();

    Task Audit(CreateAuditModel createAuditModel);
}