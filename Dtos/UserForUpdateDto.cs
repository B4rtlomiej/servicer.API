using servicer.API.Models;

namespace servicer.API.Dtos
{
    public class UserForUpdateDto
    {
        public string UserRole { get; set; }

        public Person Person { get; set; }
    }
}