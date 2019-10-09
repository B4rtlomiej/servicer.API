using System.ComponentModel.DataAnnotations;

namespace servicer.API.Models
{
    public class Person
    {
        public int Id { get; protected set; }

        public Sex Sex { get; protected set; }

        [Required, StringLength(50)]
        public string FirstName { get; protected set; }

        [Required, StringLength(50)]
        public string LastName { get; protected set; }

        [Required, StringLength(50)]
        public string Email { get; protected set; }

        [Required, StringLength(10)]
        public string Phone { get; protected set; }

        public int? UserId { get; protected set; }

        public int? CustomerId { get; protected set; }

        public Address Address { get; protected set; }

        protected Person()
        {
        }
    }
}