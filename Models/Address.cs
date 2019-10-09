using System.ComponentModel.DataAnnotations;

namespace servicer.API.Models
{
    public class Address
    {
        public int Id { get; protected set; }

        [Required, StringLength(5)]
        public string PostalCode { get; protected set; }

        [Required, StringLength(30)]
        public string City { get; protected set; }

        [Required, StringLength(50)]
        public string Street { get; protected set; }

        public int SuiteNumber { get; protected set; }

        public int? FlatNumber { get; protected set; }

        public int PersonId { get; protected set; }

        protected Address()
        {
        }
    }
}