using System;
using System.Collections.Generic;
using System.Linq;

using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace Stott.Security.Optimizely.Features.SecurityTxt;

public sealed class DefaultSecurityTxtContentRepository : ISecurityTxtContentRepository
{
    private readonly DynamicDataStore store;

    public DefaultSecurityTxtContentRepository()
    {
        store = DynamicDataStoreFactory.Instance.CreateStore(typeof(SecurityTxtEntity));
    }

    public void Delete(Guid id)
    {
        store.Delete(Identity.NewIdentity(id));
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

    public List<SecurityTxtEntity> GetAllForSite(Guid siteId)
    {
        return store.Find<SecurityTxtEntity>(new Dictionary<string, object> { { nameof(SecurityTxtEntity.SiteId), siteId } }).ToList();
    }

    public void Save(SaveSecurityTxtModel model)
    {
        var recordToSave = Get(model.Id);
        recordToSave ??= new SecurityTxtEntity
        {
            Id = Identity.NewIdentity(Guid.NewGuid()),
            SiteId = model.SiteId,
        };

        recordToSave.SpecificHost = model.SpecificHost;
        recordToSave.IsForWholeSite = string.IsNullOrWhiteSpace(model.SpecificHost);
        recordToSave.Content = model.Content;

        store.Save(recordToSave);
    }
}