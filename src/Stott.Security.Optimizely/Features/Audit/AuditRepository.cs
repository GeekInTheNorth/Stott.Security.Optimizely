namespace Stott.Security.Optimizely.Features.Audit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;

public class AuditRepository : IAuditRepository
{
    private readonly ICspDataContext _context;

    public AuditRepository(ICspDataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AuditEntry>> GetAsync(
        DateTime dateFrom, 
        DateTime dateTo, 
        string author, 
        string recordType, 
        string operationType,
        int from,
        int take)
    {
        var query = _context.AuditEntries
                            .AsQueryable()
                            .Where(x => x.Actioned >= dateFrom && x.Actioned <= dateTo);

        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(x => x.ActionedBy == author);
        }

        if (!string.IsNullOrWhiteSpace(recordType))
        {
            query = query.Where(x => x.RecordType == recordType);
        }

        if (!string.IsNullOrWhiteSpace(operationType))
        {
            query = query.Where(x => x.OperationType == operationType);
        }

        return await query.OrderByDescending(x => x.Actioned)
                          .Skip(from)
                          .Take(take)
                          .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetUsersAsync()
    {
        return await _context.AuditEntries
                             .Select(x => x.ActionedBy)
                             .Distinct()
                             .OrderBy(x => x)
                             .ToListAsync();
    }
}