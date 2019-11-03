using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public Item()
        {
        }
    }
}
