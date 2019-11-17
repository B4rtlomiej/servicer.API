using servicer.API.Models;

namespace servicer.API.Dtos
{
    public class PersonForDetailDto
    {
        public Sex Sex { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public int? UserId { get; set; }

        public int? CustomerId { get; set; }

        public Address Address { get; set; }
    }
}