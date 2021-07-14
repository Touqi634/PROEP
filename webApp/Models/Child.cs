using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace webApp.Models
{
    public class Child : User
    {
        [Required]
        public string ParentId { get; set; }
        public virtual Parent Parent { get; set; }
        public IEnumerable<TimeRestriction> TimeRestrictions { get; set; }
    }
}
