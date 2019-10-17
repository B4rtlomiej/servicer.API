using System.ComponentModel.DataAnnotations;

namespace servicer.API.Models
{
    public class Address
    {
        public int Id { get; set; }

        [Required, StringLength(5)]
        public string PostalCode { get; set; }

        [Required, StringLength(30)]
        public string City { get; set; }

        [Required, StringLength(50)]
        public string Street { get; set; }

        public int SuiteNumber { get; set; }

        public int? FlatNumber { get; set; }

        public int PersonId { get; set; }

        public Address()
        {
        }
    }
}