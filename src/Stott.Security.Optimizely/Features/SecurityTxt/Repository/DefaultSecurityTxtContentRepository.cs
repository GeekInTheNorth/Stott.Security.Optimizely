using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.Web;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Audit;
using Stott.Security.Optimizely.Features.Audit.Models;

namespace Stott.Security.Optimizely.Features.SecurityTxt.Repository;

public sealed class DefaultSecurityTxtContentRepository : ISecurityTxtContentRepository
{
    private readonly DynamicDataStore store;

    private readonly ISiteDefinitionRepository siteDefinitionRepository;

    private readonly IAuditRepository auditRepository;

    public DefaultSecurityTxtContentRepository(ISiteDefinitionRepository siteDefinitionRepository, IAuditRepository auditRepository)
    {
        store = DynamicDataStoreFactory.Instance.CreateStore(typeof(SecurityTxtEntity));
        this.siteDefinitionRepository = siteDefinitionRepository;
        this.auditRepository = auditRepository;
    }

    public async Task DeleteAsync(Guid id, string modifiedBy)
    {
        var recordToDelete = Get(id);
        if (recordToDelete is not null)
        {
            var auditModel = GetAuditModelForDelete(recordToDelete, modifiedBy);

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
        var auditModel = GetAuditModel(model, recordToSave, modifiedBy);
        recordToSave ??= new SecurityTxtEntity
        {
            Id = Identity.NewIdentity(Guid.NewGuid()),
            SiteId = model.SiteId,
        };

        recordToSave.SpecificHost = model.SpecificHost;
        recordToSave.IsForWholeSite = string.IsNullOrWhiteSpace(model.SpecificHost);
        recordToSave.Content = model.Content;

        store.Save(recordToSave);

        await auditRepository.Audit(auditModel);
    }

    private CreateAuditModel GetAuditModel(SaveSecurityTxtModel newData, SecurityTxtEntity? oldData, string modifiedBy)
    {
        string? identifier;
        if (newData.SiteId == Guid.Empty)
        {
            identifier = "All Sites";
        }
        else {
            var siteName = siteDefinitionRepository.Get(newData.SiteId);
            identifier = string.IsNullOrWhiteSpace(newData.SpecificHost)
                ? siteName?.Name ?? newData.SiteId.ToString()
                : $"{siteName?.Name ?? newData.SiteId.ToString()} - {newData.SpecificHost}";
        }

        return new CreateAuditModel
        {
            Actioned = DateTime.UtcNow,
            ActionedBy = modifiedBy,
            OperationType = oldData is null ? "Create" : "Update",
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
                    PropertyName = nameof(SecurityTxtEntity.SiteId),
                    OriginalValue = ToAuditString(oldData?.SiteId),
                    NewValue = ToAuditString(newData.SiteId)
                }
            }
        };
    }

    private CreateAuditModel GetAuditModelForDelete(SecurityTxtEntity oldData, string modifiedBy)
    {
        string? identifier;
        if (oldData.SiteId == Guid.Empty)
        {
            identifier = "All Sites";
        }
        else {
            var siteName = siteDefinitionRepository.Get(oldData.SiteId);
            identifier = string.IsNullOrWhiteSpace(oldData.SpecificHost)
                ? siteName?.Name ?? oldData.SiteId.ToString()
                : $"{siteName?.Name ?? oldData.SiteId.ToString()} - {oldData.SpecificHost}";
        }

        return new CreateAuditModel
        {
            Actioned = DateTime.UtcNow,
            ActionedBy = modifiedBy,
            OperationType = "Delete",
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
                    PropertyName = nameof(SecurityTxtEntity.SiteId),
                    OriginalValue = ToAuditString(oldData.SiteId),
                    NewValue = null
                }
            }
        };
    }

    private static string? ToAuditString(Guid? value)
    {
        return value == null || Guid.Empty.Equals(value) ? "All Sites" : value.ToString();
    }
}