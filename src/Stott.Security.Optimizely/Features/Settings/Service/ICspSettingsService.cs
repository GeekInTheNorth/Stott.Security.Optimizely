﻿namespace Stott.Security.Optimizely.Features.Settings.Service;

using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;

public interface ICspSettingsService
{
    CspSettings Get();

    Task<CspSettings> GetAsync();

    Task SaveAsync(ICspSettings cspSettings, string? modifiedBy);
}