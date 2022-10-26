namespace Stott.Security.Optimizely.Features.Audit;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;

public interface IAuditRepository
{
    Task<IEnumerable<AuditEntry>> GetAsync(
        DateTime dateFrom, 
        DateTime dateTo, 
        string author, 
        string recordType, 
        string operationType,
        int from,
        int take);

    Task<IEnumerable<string>> GetUsersAsync();
}
