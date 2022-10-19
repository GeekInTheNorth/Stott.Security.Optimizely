namespace Stott.Security.Optimizely.Features.Audit;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;

public interface IAuditRepository
{
    Task<IEnumerable<AuditEntry>> Get(DateTime from, DateTime to, string author, string recordType, string operationType);
}
