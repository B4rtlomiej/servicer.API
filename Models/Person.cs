using System.ComponentModel.DataAnnotations;

namespace servicer.API.Models
{
    public class Person
    {
        public int Id { get; set; }

        public Sex Sex { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        [Required, StringLength(50)]
        public string Email { get; set; }

        [Required, StringLength(10)]
        public string Phone { get; set; }

        public int? UserId { get; set; }

        public int? CustomerId { get; set; }

        public Address Address { get; set; }

        public Person()
        {
        }
    }
}