using System;
using servicer.API.Models;

namespace servicer.API.Dtos
{
    public class UserForDetailDto
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActive { get; set; }

        public string UserRole { get; set; }

        public PersonForDetailDto Person { get; protected set; }
    }
}