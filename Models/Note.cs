using System.ComponentModel.DataAnnotations;

namespace servicer.API.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public ProductSpecification ProductSpecification { get; set; }

        public int? ProductSpecificationId { get; set; }

        public Item Item { get; set; }

        public int? ItemId { get; set; }

        public Customer Customer { get; set; }

        public int? CustomerId { get; set; }

        public Note()
        {
        }
    }
}
