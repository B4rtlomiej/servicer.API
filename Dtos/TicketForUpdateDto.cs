using System;

namespace servicer.API.Dtos
{
    public class TicketForUpdateDto
    {
        public string Origin { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime Closed { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
    }
}