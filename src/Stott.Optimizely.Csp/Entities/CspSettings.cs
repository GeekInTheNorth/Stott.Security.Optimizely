using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stott.Optimizely.Csp.Entities
{
    [Table("tbl_CspSettings")]
    public class CspSettings
    {
        public Guid Id { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsReportOnly { get; set; }
    }
}
