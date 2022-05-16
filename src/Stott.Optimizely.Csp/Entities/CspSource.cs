using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stott.Optimizely.Csp.Entities
{
    [Table("tbl_CspSource")]
    public class CspSource
    {
        public Guid Id { get; set; }

        [StringLength(250)]
        public string Source { get; set; }

        [StringLength(1000)]
        public string Directives { get; set; }
    }
}
