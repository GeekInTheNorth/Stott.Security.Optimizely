using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EPiServer.Data;
using EPiServer.Data.Dynamic;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Applications;
using Stott.Security.Optimizely.Features.Audit;
using Stott.Security.Optimizely.Features.Audit.Models;
using Stott.Security.Optimizely.Features.SecurityTxt.Models;

namespace Stott.Security.Optimizely.Features.SecurityTxt.Repository;

public sealed class DefaultSecurityTxtContentRepository : ISecurityTxtContentRepository
{
    private readonly DynamicDataStore store;

    private readonly IApplicationDefinitionService appService;

    private readonly IAuditRepository auditRepository;

    public DefaultSecurityTxtContentRepository(IApplicationDefinitionService appService, IAuditRepository auditRepository)
    {
        store = DynamicDataStoreFactory.Instance.CreateStore(typeof(SecurityTxtEntity));
        this.appService = appService;
        this.auditRepository = auditRepository;
    }

    public async Task DeleteAsync(Guid id, string modifiedBy)
    {
        var recordToDelete = Get(id);
        if (recordToDelete is not null)
        {
            var auditModel = await GetAuditModelForDelete(recordToDelete, modifiedBy);

            store.Delete(Identity.NewIdentity(id));

            await auditRepository.Audit(auditModel);
        }
    }

    public SecurityTxtEntity? Get(Guid id)
    {
        if (Guid.Empty.Equals(id))
        {
            return null;
        }

        return store.Load<SecurityTxtEntity>(Identity.NewIdentity(id));
    }

    public List<SecurityTxtEntity> GetAll()
    {
        return store.Find<SecurityTxtEntity>(new Dictionary<string, object>()).ToList();
    }

    public async Task SaveAsync(SaveSecurityTxtModel model, string modifiedBy)
    {
        var recordToSave = Get(model.Id);
        var auditModel = await GetAuditModel(model, recordToSave, modifiedBy);
        recordToSave ??= new SecurityTxtEntity
        {
            Id = Identity.NewIdentity(Guid.NewGuid()),
            AppId = model.AppId,
        };

        recordToSave.SpecificHost = model.SpecificHost;
        recordToSave.IsForWholeSite = string.IsNullOrWhiteSpace(model.SpecificHost);
        recordToSave.Content = model.Content;

        store.Save(recordToSave);

        await auditRepository.Audit(auditModel);
    }

    private async Task<CreateAuditModel> GetAuditModel(SaveSecurityTxtModel newData, SecurityTxtEntity? oldData, string modifiedBy)
    {
        string? identifier;
        if (string.IsNullOrWhiteSpace(newData.AppId))
        {
            identifier = "All Sites";
        }
        else {
            var siteName = await appService.GetApplicationByIdAsync(newData.AppId);
            identifier = string.IsNullOrWhiteSpace(newData.SpecificHost)
                ? siteName?.AppName ?? newData.AppId
                : $"{siteName?.AppName ?? newData.AppId} - {newData.SpecificHost}";
        }

        return new CreateAuditModel
        {
            Actioned = DateTime.UtcNow,
            ActionedBy = modifiedBy,
            OperationType = oldData is null ? EntityState.Added.ToString() : EntityState.Modified.ToString(),
            RecordType = "security.txt",
            Identifier = identifier,
            Changes = new List<CreateAuditModel.CreateAuditItem>
            {
                new()
                {
                    PropertyName = nameof(SecurityTxtEntity.SpecificHost),
                    OriginalValue = oldData?.SpecificHost,
                    NewValue = newData.SpecificHost
                },
                new()
                {
                    PropertyName = nameof(SecurityTxtEntity.Content),
                    OriginalValue = oldData?.Content,
                    NewValue = newData.Content
                },
                new()
                {
                    PropertyName = nameof(SecurityTxtEntity.AppId),
                    OriginalValue = ToAuditString(oldData?.AppId),
                    NewValue = ToAuditString(newData.AppId)
                }
            }
        };
    }

    private async Task<CreateAuditModel> GetAuditModelForDelete(SecurityTxtEntity oldData, string modifiedBy)
    {
        string? identifier;
        if (string.IsNullOrWhiteSpace(oldData.AppId))
        {
            identifier = "All Sites";
        }
        else {
            var siteName = await appService.GetApplicationByIdAsync(oldData.AppId);
            identifier = string.IsNullOrWhiteSpace(oldData.SpecificHost)
                ? siteName?.AppName ?? oldData.AppId
                : $"{siteName?.AppName ?? oldData.AppId} - {oldData.SpecificHost}";
        }

        return new CreateAuditModel
        {
            Actioned = DateTime.UtcNow,
            ActionedBy = modifiedBy,
            OperationType = EntityState.Deleted.ToString(),
            RecordType = "security.txt",
            Identifier = identifier,
            Changes = new List<CreateAuditModel.CreateAuditItem>
            {
                new()
                {
                    PropertyName = nameof(SecurityTxtEntity.SpecificHost),
                    OriginalValue = oldData.SpecificHost,
                    NewValue = null
                },
                new()
                {
                    PropertyName = nameof(SecurityTxtEntity.Content),
                    OriginalValue = oldData.Content,
                    NewValue = null
                },
                new()
                {
                    PropertyName = nameof(SecurityTxtEntity.AppId),
                    OriginalValue = ToAuditString(oldData.AppId),
                    NewValue = null
                }
            }
        };
    }

    private static string? ToAuditString(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "All Sites" : value;
    }
}