using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace servicer.API.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(50)]
        public string Manufacturer { get; set; }

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
