namespace Stott.Security.Optimizely.Features.Csp.AllowList;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Common;

internal sealed class AllowListRepository : IAllowListRepository
{
    private readonly IHttpClientFactory _clientFactory;

    public AllowListRepository(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
    }

    public async Task<AllowListCollection> GetAllowListAsync(string allowListUrl)
    {
        if (!Uri.IsWellFormedUriString(allowListUrl, UriKind.Absolute))
        {
            throw new ArgumentException($"{CspConstants.LogPrefix} {nameof(allowListUrl)} should be a valid url.", nameof(allowListUrl));
        }

        var allowListEntries = await GetRemoteAllowListAsync(allowListUrl);

        return new AllowListCollection(allowListEntries);
    }

    private async Task<IList<AllowListEntry>> GetRemoteAllowListAsync(string allowListUrl)
    {
        var client = _clientFactory.CreateClient();
        var response = await client.GetAsync(allowListUrl);

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();

            var serializationOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Deserialize<List<AllowListEntry>>(responseData, serializationOptions) ?? new List<AllowListEntry>();
        }

        return new List<AllowListEntry>(0);
    }
}