using System;
using System.ComponentModel.DataAnnotations;
using servicer.API.Models;
using Type = servicer.API.Models.Type;

namespace servicer.API.Dtos
{
    public class TicketForListDto
    {
        public int Id { get; set; }
        public Origin Origin { get; set; }
        public Type Type { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public DateTime Created { get; set; }
        public DateTime Closed { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int ItemId { get; set; }
    }
}