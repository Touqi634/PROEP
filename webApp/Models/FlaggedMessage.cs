using System.ComponentModel.DataAnnotations;

namespace webApp.Models
{
    public class FlaggedMessage
    {
        [Key]
        public int FlaggedMessageId { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public string SenderID { get; set; }
        public User Sender { get; set; }
        public string Reason { get; set; }
    }
}
