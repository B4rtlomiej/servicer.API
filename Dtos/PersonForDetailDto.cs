using System.Collections.Generic;

namespace servicer.API.Dtos
{
    public class PersonForDetailDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public int? UserId { get; set; }

        public int? CustomerId { get; set; }

        public ICollection<NoteForDetailDto> CustomerNotes { get; set; }
    }
}