using System;
using System.ComponentModel.DataAnnotations;

namespace servicer.API.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public Origin Origin { get; set; }

        public Type Type { get; set; }

        public Status Status { get; set; }

        public Priority Priority { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime Closed { get; set; }

        [Required, StringLength(100)]
        public string Subject { get; set; }

        [Required]
        public string Description { get; set; }

        public Item Item { get; set; }

        public int ItemId { get; set; }

        public User User { get; set; }

        public int? UserId { get; set; }

        public Ticket()
        {
        }
    }
}
