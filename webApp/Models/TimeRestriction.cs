using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApp.Models
{
    public class TimeRestriction : DataModel
    {
        [Key]
        public int RestrictionId { get; set; }
        public Child RestrictedUser { get; set; }
        [Required]
        public string RestrictedUserId { get; set; }
        [ForeignKey("RestrictedUserId")]
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DayOfWeek Day { get; set; }

    }
}
