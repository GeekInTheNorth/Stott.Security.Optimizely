using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace Stott.Optimizely.Csp.Entities
{
    public class CspSource : IDynamicData
    {
        public Identity Id { get; set; }

        public string Source { get; set; }

        public string Directives { get; set; }
    }
}
