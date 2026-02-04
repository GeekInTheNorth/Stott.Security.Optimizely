namespace Stott.Security.Optimizely.Features.CustomHeaders.Repository;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.CustomHeaders.Models;

/// <summary>
/// Mapper for converting between CustomHeader entities and models.
/// </summary>
internal static class CustomHeaderMapper
{
    /// <summary>
    /// Maps a CustomHeader entity to a CustomHeaderModel.
    /// </summary>
    internal static CustomHeaderModel ToModel(CustomHeader entity)
    {
        return new CustomHeaderModel
        {
            Id = entity.Id,
            HeaderName = entity.HeaderName,
            Behavior = entity.Behavior,
            HeaderValue = entity.HeaderValue
        };
    }

    /// <summary>
    /// Maps a SaveCustomHeaderModel to a CustomHeader entity.
    /// </summary>
    internal static void ToEntity(ICustomHeader model, CustomHeader entity)
    {
        entity.HeaderName = model.HeaderName;
        entity.Behavior = model.Behavior;
        entity.HeaderValue = model.HeaderValue;
    }
}
