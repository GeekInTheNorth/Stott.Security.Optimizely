using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Common;

namespace Stott.Security.Optimizely.Features.Whitelist
{
    public class WhitelistRepository : IWhitelistRepository
    {
        private readonly IHttpClientFactory _clientFactory;

        public WhitelistRepository(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<WhitelistCollection> GetWhitelistAsync(string whitelistUrl)
        {
            if (!Uri.IsWellFormedUriString(whitelistUrl, UriKind.Absolute))
            {
                throw new ArgumentException($"{CspConstants.LogPrefix} {nameof(whitelistUrl)} should be a valid url.", nameof(whitelistUrl));
            }

            var whitelistEntries = await GetRemoteWhitelistAsync(whitelistUrl);

            return new WhitelistCollection(whitelistEntries);
        }

        private async Task<IList<WhitelistEntry>> GetRemoteWhitelistAsync(string whitelistUrl)
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync(whitelistUrl);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();

                var serializationOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                return JsonSerializer.Deserialize<List<WhitelistEntry>>(responseData, serializationOptions) ?? new List<WhitelistEntry>();
            }

            return new List<WhitelistEntry>(0);
        }
    }
}
