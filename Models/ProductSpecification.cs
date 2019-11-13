using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace servicer.API.Models
{
    public class ProductSpecification
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Manufacturer { get; set; }

        [Required]
        [StringLength(50)]
        public string Series { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Note> Notes { get; set; }

        public ICollection<Item> Items { get; set; }

        public ProductSpecification()
        {
        }
    }
}
