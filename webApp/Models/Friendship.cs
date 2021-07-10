using System.ComponentModel.DataAnnotations.Schema;

namespace webApp.Models
{
    public class Friendship : DataModel
    {
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string FriendId { get; set; }
        [ForeignKey("FriendId")]
        public User Friend { get; set; }
        public bool IsBlocked { get; set; }
    }
}
