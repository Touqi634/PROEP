using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApp.Models
{
    public class User : DataModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Bio { get; set; }
        public IEnumerable<Friendship> Friends { get; set; } 
        public IEnumerable<Message> Messages { get; set; }
    }
}
