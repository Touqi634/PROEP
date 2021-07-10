using System.ComponentModel.DataAnnotations;

namespace webApp.Models
{
    public class Child : User
    {
        public string ParentId { get; set; }
        [Required]
        public virtual Parent Parent { get; set; }
    }
}
