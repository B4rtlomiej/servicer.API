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
        public DateTime Created { get; protected set; }

        [Required]
        public DateTime LastActive { get; protected set; }

        public UserRole UserRole { get; protected set; }

        public Person Person { get; protected set; }

        public User()
        {
        }
    }
}