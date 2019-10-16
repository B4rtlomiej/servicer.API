using System;
using System.ComponentModel.DataAnnotations;
using servicer.API.Models;

namespace servicer.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Nazwa użytkowika musi mieć od 6 do 50 znaków.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [MinLength(4, ErrorMessage = "Hasło musi mieć co najmniej 4 znaki.")]
        public string Password { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActive { get; set; }

        public Person Person { get; set; }

        public UserForRegisterDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}