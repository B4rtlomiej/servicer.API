using System;
using System.ComponentModel.DataAnnotations;
using servicer.API.Models;
using Type = servicer.API.Models.Type;

namespace servicer.API.Dtos
{
    public class TicketForSendDto
    {
        public Origin Origin { get; set; }
        public Type Type { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public DateTime Created { get; set; }
        public DateTime Closed { get; set; }

        [Required(ErrorMessage = "Temat jest wymagany.")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Temat musi zawierać co najmniej 10 znaków.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Opis jest wymagany.")]
        [MinLength(4, ErrorMessage = "Opis musi mieć co najmniej 20 znaków.")]
        public string Description { get; set; }
        public Item Item { get; set; }

        public TicketForSendDto()
        {
            this.Created = DateTime.Now;
            this.Status = Status.New;
        }
    }
}