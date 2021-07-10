using System.ComponentModel.DataAnnotations;

namespace webApp.Models
{
    public class Message : DataModel
    {
        [Key]
        public int MessageId { get; set; }
        public MessageStatus Status { get; set; }
        public string Content { get; set; }
        public string SenderID { get; set; }
        public User Sender { get; set; }
        public string ReceiverId { get; set; }
        public User Receiver { get; set; }
    }

    public enum MessageStatus
    {
        Sent,
        Delivered,
        Seen
    }
}
