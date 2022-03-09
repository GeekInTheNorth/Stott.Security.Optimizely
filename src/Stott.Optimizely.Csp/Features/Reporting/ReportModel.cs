namespace Stott.Optimizely.Csp.Features.Reporting
{
    public class ReportModel
    {
        public string DocumentUri { get; set; }

        public string Referrer { get; set; }

        public string BlockedUri { get; set; }

        public string EffectiveDirective { get; set; }

        public string ViolatedDirective { get; set; }

        public string OriginalPolicy { get; set; }

        public string Disposition { get; set; }

        public string ScriptSample { get; set; }

        public string SourceFile { get; set; }
    }
}
