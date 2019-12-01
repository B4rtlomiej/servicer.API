using System;
using System.Collections.Generic;
using servicer.API.Models;

namespace servicer.API.Dtos
{
    public class TicketForDetailDto
    {
        public int Id { get; set; }
        public string Origin { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime Created { get; set; }
        public DateTime Closed { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public ICollection<NoteForDetailDto> TicketNotes { get; set; }
        public ICollection<NoteForDetailDto> CustomerNotes { get; set; }
        public ICollection<NoteForDetailDto> ItemNotes { get; set; }
        public ICollection<NoteForDetailDto> ProductSpecificationNotes { get; set; }
        public Item Item { get; set; }
        public User User { get; set; }
    }
}