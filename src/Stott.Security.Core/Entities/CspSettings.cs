using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stott.Security.Core.Entities
{
    [Table("tbl_CspSettings")]
    public class CspSettings
    {
        public Guid Id { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsReportOnly { get; set; }

        public bool IsWhitelistEnabled { get; set; }

        public string WhitelistUrl { get; set; }
    }
}
