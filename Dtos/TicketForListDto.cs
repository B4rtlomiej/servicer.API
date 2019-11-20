using System;
using servicer.API.Models;

namespace servicer.API.Dtos
{
    public class TicketForListDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime Created { get; set; }
        public string Subject { get; set; }
        public Item Item { get; set; }
        public User User { get; set; }
    }
}