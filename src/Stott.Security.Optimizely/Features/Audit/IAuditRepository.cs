namespace Stott.Security.Optimizely.Features.Audit;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;

public interface IAuditRepository
{
    Task<IEnumerable<AuditEntry>> GetAsync(DateTime from, DateTime to, string author, string recordType, string operationType);

    Task<IEnumerable<string>> GetUsersAsync();
}
