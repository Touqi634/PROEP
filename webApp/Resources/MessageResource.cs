using System;
using webApp.Models;

namespace webApp.Resources
{
    public class MessageResource
    {
        public int MessageId { get; set; }
        public MessageStatus Status { get; set; }
        public string Content { get; set; }
        public UserResource Sender { get; set; }
        public UserResource Receiver { get; set; }
    }
}
