namespace Stott.Security.Optimizely.Features.Audit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Audit.Models;

internal sealed class AuditRepository : IAuditRepository
{
    private readonly Lazy<ICspDataContext> _context;

    public AuditRepository(Lazy<ICspDataContext> context)
    {
        _context = context;
    }

    public async Task Audit(CreateAuditModel? model)
    {
        if (model is not { Changes.Count: > 0 })
        {
            return;
        }

        var changes = model.Changes.Where(x => x.HasChanged).ToList();
        if (changes.Count == 0)
        {
            return;
        }

        var parent = new AuditHeader
        {
            RecordType = model.RecordType,
            OperationType = model.OperationType,
            Actioned = model.Actioned,
            ActionedBy = model.ActionedBy,
            Identifier = model.Identifier
        };

        _context.Value.AuditHeaders.Add(parent);

        foreach (var change in changes)
        {
            var child = new AuditProperty
            {
                Header = parent,
                Field = change.PropertyName,
                OldValue = change.OriginalValue,
                NewValue = change.NewValue
            };
            _context.Value.AuditProperties.Add(child);
        }

        await _context.Value.SaveChangesAsync();
    }

    public async Task<IEnumerable<AuditEntryModel>> GetAsync(
        DateTime dateFrom,
        DateTime dateTo,
        string? author,
        string? recordType,
        string? operationType,
        int from,
        int take,
        string? searchTerm)
    {
        var startOfDateFrom = dateFrom.Date;
        var endOfDateTo = dateTo.Date.AddDays(1).AddMilliseconds(-1);

        var query = _context.Value
                            .AuditHeaders
                            .Include(x => x.AuditProperties)
                            .AsQueryable()
                            .Where(x => x.Actioned >= startOfDateFrom && x.Actioned <= endOfDateTo);

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

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(x => x.Identifier.Contains(searchTerm) ||
                                     x.AuditProperties.Any(p => p.OldValue.Contains(searchTerm) || p.NewValue.Contains(searchTerm)));
        }

        return await query.OrderByDescending(x => x.Actioned)
                          .ThenBy(x => x.Id)
                          .Skip(from)
                          .Take(take)
                          .Select(x => new AuditEntryModel
                          {
                              Id = x.Id,
                              Actioned = x.Actioned,
                              ActionedBy = x.ActionedBy,
                              OperationType = x.OperationType,
                              RecordType = x.RecordType,
                              Identifier = x.Identifier,
                              Changes = x.AuditProperties.Select(y => new AuditEntryItemModel
                              {
                                  Id = y.Id,
                                  Field = y.Field,
                                  OldValue = y.OldValue,
                                  NewValue = y.NewValue
                              })
                          })
                          .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetUsersAsync()
    {
        return await _context.Value
                             .AuditHeaders
                             .Select(x => x.ActionedBy)
                             .Distinct()
                             .OrderBy(x => x)
                             .ToListAsync();
    }
}