using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Features.Header
{
    public class CspContentBuilder : ICspContentBuilder
    {
        private bool _sendViolationReport;

        private string _violationReportUrl;

        private List<CspSourceDto> _cspSources;

        public ICspContentBuilder WithSources(IEnumerable<CspSource> sources)
        {
            _cspSources = ConvertToDtos(sources).ToList();

            return this;
        }

        public ICspContentBuilder WithReporting(bool sendViolationReport, string violationReportUrl)
        {
            _sendViolationReport = sendViolationReport;
            _violationReportUrl = violationReportUrl;

            return this;
        }

        public string Build()
        {
            if (_cspSources == null || !_cspSources.Any())
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();
            var distinctDirectives = _cspSources.SelectMany(x => x.Directives)
                                                .Distinct()
                                                .ToList();

            foreach(var directive in distinctDirectives)
            {
                var directiveSources = _cspSources.Where(x => x.Directives.Contains(directive))
                                                  .Select(x => x.Source?.ToLower())
                                                  .OrderBy(x => GetSortIndex(x))
                                                  .ThenBy(x => x)
                                                  .Distinct();

                stringBuilder.Append($"{directive} {string.Join(" ", directiveSources)}; ");
            }

            if (_sendViolationReport && !string.IsNullOrWhiteSpace(_violationReportUrl))
            {
                stringBuilder.Append($"report-to {_violationReportUrl};");
            }

            return stringBuilder.ToString().Trim();
        }

        private IEnumerable<CspSourceDto> ConvertToDtos(IEnumerable<CspSource> sources)
        {
            if (sources == null)
            {
                yield break;
            }

            foreach(var source in sources)
            {
                var dto = new CspSourceDto
                {
                    Source = source.Source,
                    Directives = source.Directives?.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList()
                };

                if (dto.Directives != null && dto.Directives.Any())
                {
                    yield return dto;
                }
            }
        }

        private int GetSortIndex(string source)
        {
            var index = CspConstants.AllSources.IndexOf(source);

            return index < 0 ? 100 : index;
        }

        private class CspSourceDto
        {
            public string Source { get; set; }

            public List<string> Directives { get; set; }
        }
    }
}
