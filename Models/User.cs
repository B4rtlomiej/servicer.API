using System;
using System.ComponentModel.DataAnnotations;

namespace servicer.API.Models
{
    public class User
    {
        public int Id { get; protected set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public DateTime LastActive { get; set; }

        public UserRole UserRole { get; set; }

        public Person Person { get; set; }

        public User()
        {
        }
    }
}