using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stott.Security.Core.Entities
{
    [Table("tbl_CspSource")]
    public class CspSource
    {
        public Guid Id { get; set; }

        public string Source { get; set; }

        public string Directives { get; set; }
    }
}
