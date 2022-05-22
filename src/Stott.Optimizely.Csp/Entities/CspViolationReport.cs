using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Stott.Optimizely.Csp.Entities
{
    [Table("tbl_CspViolationReport")]
    [Index(nameof(BlockedUri), nameof(ViolatedDirective), nameof(Reported), IsUnique = false, Name = "idx_CspViolationReport_Reported")]
    public class CspViolationReport
    {
        public Guid Id { get; set; }

        public DateTime Reported { get; set; }

        [MaxLength(1000)]
        public string BlockedUri { get; set; }

        public string BlockedQueryString { get; set; }

        public string Disposition { get; set; }

        public string DocumentUri { get; set; }

        [MaxLength(100)]
        public string EffectiveDirective { get; set; }

        public string OriginalPolicy { get; set; }

        public string Referrer { get; set; }

        public string ScriptSample { get; set; }

        public string SourceFile { get; set; }

        [MaxLength(100)]
        public string ViolatedDirective { get; set; }
    }
}
