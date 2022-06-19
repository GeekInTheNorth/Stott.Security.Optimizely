namespace Stott.Security.Core.Features.Permissions.Service;

using Stott.Security.Core.Common;
using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.Caching;
using Stott.Security.Core.Features.Permissions.Repository;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CspPermissionService : ICspPermissionService
{
    private readonly ICspPermissionRepository _repository;

    private readonly ICacheWrapper _cacheWrapper;

    public CspPermissionService(
        ICspPermissionRepository repository, 
        ICacheWrapper cacheWrapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _cacheWrapper = cacheWrapper ?? throw new ArgumentNullException(nameof(cacheWrapper));
    }

    public async Task AppendDirectiveAsync(string source, string directive)
    {
        await _repository.AppendDirectiveAsync(source, directive);

        _cacheWrapper.Remove(CspConstants.CacheKeys.CompiledCsp);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);

        _cacheWrapper.Remove(CspConstants.CacheKeys.CompiledCsp);
    }

    public async Task<IList<CspSource>> GetAsync()
    {
        return await _repository.GetAsync();
    }

    public IList<CspSource> GetCmsRequirements()
    {
        return _repository.GetCmsRequirements();
    }

    public async Task SaveAsync(Guid id, string source, List<string> directives)
    {
        await _repository.SaveAsync(id, source, directives);

        _cacheWrapper.Remove(CspConstants.CacheKeys.CompiledCsp);
    }
}
