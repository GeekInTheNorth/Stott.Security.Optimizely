using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Stott.Security.Optimizely.Entities
{
    [Table("tbl_CspViolationSummary")]
    [Index(nameof(BlockedUri), nameof(ViolatedDirective), IsUnique = false, Name = "idx_CspViolationSummary_LookUp")]
    public class CspViolationSummary
    {
        public Guid Id { get; set; }

        [MaxLength(1000)]
        public string BlockedUri { get; set; }

        [MaxLength(100)]
        public string ViolatedDirective { get; set; }

        public DateTime LastReported { get; set; }

        public int Instances { get; set; }
    }
}
