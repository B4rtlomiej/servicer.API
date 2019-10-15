using System;
using servicer.API.Models;

namespace servicer.API.Dtos
{
    public class UserForListDto
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public DateTime LastActive { get; set; }

        public String UserRole { get; set; }

        public Person Person { get; set; }
    }
}