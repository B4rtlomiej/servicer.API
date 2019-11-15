using System;
using System.Collections.Generic;

namespace servicer.API.Models
{
    public class Item
    {
        public int Id { get; set; }

        public DateTime ProductionYear { get; set; }

        public ProductSpecification ProductSpecification { get; set; }

        public int ProductSpecificationId { get; set; }

        public Customer Customer { get; set; }

        public int CustomerId { get; set; }

        public ICollection<Ticket> Tickets { get; set; }

        public ICollection<Note> Notes { get; set; }

        public Item()
        {
        }
    }
}
