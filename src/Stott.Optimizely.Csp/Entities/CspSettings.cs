using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace Stott.Optimizely.Csp.Entities
{
    public class CspSettings : IDynamicData
    {
        public Identity Id { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsReportOnly { get; set; }
    }
}
