using System;
using servicer.API.Models;
using Type = servicer.API.Models.Type;

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
        public Item Item { get; set; }
    }
}