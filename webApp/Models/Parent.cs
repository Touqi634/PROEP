using System.Collections.Generic;

namespace webApp.Models
{
    public class Parent : User
    {
        public ICollection<Child> Children { get; set; }
    }
}
