namespace Stott.Security.Optimizely.Features.Guides.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Guides.Models;

internal sealed class GuideService(
    IHttpClientFactory clientFactory,
    ICacheWrapper cache,
    ILogger<IGuideService> logger) : IGuideService
{
    private const string GuidesFeedUrl = "https://www.stott.pro/data/stott-security.json";

    public async Task<IList<GuideModel>> GetGuidesAsync()
    {
        var cachedGuides = cache.Get<List<GuideModel>>(CspConstants.CacheKeys.Guides);
        if (cachedGuides is not null)
        {
            return cachedGuides;
        }

        var guides = await GetRemoteGuidesAsync();

        cache.Add(CspConstants.CacheKeys.Guides, guides);

        return guides;
    }

    private async Task<List<GuideModel>> GetRemoteGuidesAsync()
    {
        try
        {
            var client = clientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            var response = await client.GetAsync(GuidesFeedUrl);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError(
                    "{LogPrefix} Failed to retrieve guides from {Url}. Status code was {StatusCode}.",
                    CspConstants.LogPrefix,
                    GuidesFeedUrl,
                    response.StatusCode);

                return new List<GuideModel>(0);
            }

            var responseData = await response.Content.ReadAsStringAsync();

            var serializationOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var guides = JsonSerializer.Deserialize<List<GuideModel>>(responseData, serializationOptions) ?? new List<GuideModel>(0);

            return guides.OrderByDescending(x => x.Date).ToList();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{LogPrefix} Failed to retrieve guides from {Url}.", CspConstants.LogPrefix, GuidesFeedUrl);

            return new List<GuideModel>(0);
        }
    }
}
