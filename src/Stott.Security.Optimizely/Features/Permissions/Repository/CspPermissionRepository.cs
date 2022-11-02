namespace Stott.Security.Optimizely.Features.Permissions.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Entities.Exceptions;

public class CspPermissionRepository : ICspPermissionRepository
{
    private readonly ICspDataContext _cspDataContext;

    public CspPermissionRepository(ICspDataContext cspDataContext)
    {
        _cspDataContext = cspDataContext;
    }

    public async Task<IList<CspSource>> GetAsync()
    {
        var sources = await _cspDataContext.CspSources.ToListAsync();

        return sources ?? new List<CspSource>(0);
    }

    public IList<CspSource> GetCmsRequirements()
    {
        var selfRequirements = new List<string>
        {
            CspConstants.Directives.ChildSource,
            CspConstants.Directives.ConnectSource,
            CspConstants.Directives.DefaultSource,
            CspConstants.Directives.FrameSource,
            CspConstants.Directives.ImageSource,
            CspConstants.Directives.ScriptSource,
            CspConstants.Directives.StyleSource,
            CspConstants.Directives.FontSource
        };

        var unsafeInlineRequirements = new List<string>
        {
            CspConstants.Directives.ScriptSource,
            CspConstants.Directives.StyleSource
        };

        var applicationInsightsRequirements = new List<string>
        {
            CspConstants.Directives.ConnectSource,
            CspConstants.Directives.ScriptSource
        };

        return new List<CspSource>
        {
            new CspSource { Source = CspConstants.Sources.Self, Directives = string.Join(",", selfRequirements) },
            new CspSource { Source = CspConstants.Sources.UnsafeInline, Directives = string.Join(",", unsafeInlineRequirements) },
            new CspSource { Source = CspConstants.Sources.UnsafeEval, Directives = CspConstants.Directives.ScriptSource },
            new CspSource { Source = "https://dc.services.visualstudio.com", Directives = string.Join(",", applicationInsightsRequirements)  },
            new CspSource { Source = "https://*.msecnd.net", Directives = CspConstants.Directives.ScriptSource }
        };
    }

    public async Task DeleteAsync(Guid id, string deletedBy)
    {
        var existingRecord = await _cspDataContext.CspSources.FirstOrDefaultAsync(x => x.Id == id);
        if (existingRecord != null)
        {
            existingRecord.Modified = DateTime.UtcNow;
            existingRecord.ModifiedBy = deletedBy;

            _cspDataContext.CspSources.Remove(existingRecord);
            await _cspDataContext.SaveChangesAsync();
        }
    }

    public async Task SaveAsync(Guid id, string source, List<string> directives, string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (directives == null || !directives.Any())
        {
            throw new ArgumentException($"{directives} must not be null or empty.", nameof(directives));
        }

        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            throw new ArgumentNullException(nameof(modifiedBy));
        }

        var combinedDirectives = string.Join(",", directives);
        await SaveAsync(id, source, combinedDirectives, modifiedBy);
    }

    public async Task AppendDirectiveAsync(string source, string directive, string modifiedBy)
    {
        var matchingSource = await _cspDataContext.CspSources.FirstOrDefaultAsync(x => x.Source == source);

        if (matchingSource == null)
        {
            _cspDataContext.CspSources.Add(new CspSource
            {
                Source = source,
                Directives = directive,
                Modified = DateTime.UtcNow,
                ModifiedBy = modifiedBy
            });
        }
        else if (!matchingSource.Directives.Contains(directive))
        {
            matchingSource.Directives = $"{matchingSource.Directives},{directive}";
            matchingSource.Modified = DateTime.UtcNow;
            matchingSource.ModifiedBy = modifiedBy;
        }

        await _cspDataContext.SaveChangesAsync();
    }

    private async Task SaveAsync(Guid id, string source, string directives, string modifiedBy)
    {
        var matchingSource = await _cspDataContext.CspSources.FirstOrDefaultAsync(x => x.Source == source);
        if (matchingSource != null && !matchingSource.Id.Equals(id))
        {
            throw new EntityExistsException($"{CspConstants.LogPrefix} An entry already exists for the source of '{source}'.");
        }

        var recordToSave = await _cspDataContext.CspSources.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (recordToSave == null)
        {
            recordToSave = new CspSource
            {
                Source = source,
                Directives = directives
            };

            _cspDataContext.CspSources.Add(recordToSave);
        }

        recordToSave.Source = source;
        recordToSave.Directives = directives;
        recordToSave.Modified = DateTime.UtcNow;
        recordToSave.ModifiedBy = modifiedBy;

        await _cspDataContext.SaveChangesAsync();
    }
}
