using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stott.Optimizely.Csp.Entities
{
    [Table("tbl_CspViolationReport")]
    public class CspViolationReport
    {
        public Guid Id { get; set; }

        public DateTime Reported { get; set; }

        public string BlockedUri { get; set; }

        public string Disposition { get; set; }

        public string DocumentUri { get; set; }

        public string EffectiveDirective { get; set; }

        public string OriginalPolicy { get; set; }

        public string Referrer { get; set; }

        public string ScriptSample { get; set; }

        public string SourceFile { get; set; }

        public string ViolatedDirective { get; set; }
    }
}
