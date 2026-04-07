using System;

using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace Stott.Security.Optimizely.Entities;

[EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
public class SecurityTxtEntity : IDynamicData
{
    public Identity Id { get; set; } = Identity.NewIdentity(Guid.NewGuid());

    public string? AppId { get; set; }

    public bool IsForWholeSite { get; set; }

    public string? SpecificHost { get; set; }

    public string? Content { get; set; }
}