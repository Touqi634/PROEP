using System;
using System.ComponentModel.DataAnnotations;

namespace webApp.Models
{
    public class DataModel
    {
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }
    }
}
