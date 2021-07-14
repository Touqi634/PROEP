using System;

namespace webApp.Resources
{
    public class FlaggedMessageResource
    {
        public int FlaggedMessageId { get; set; }
        public string Content { get; set; }
        public UserResource Sender { get; set; }
        public string Reason { get; set; }
    }
}
