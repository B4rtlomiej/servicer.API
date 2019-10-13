namespace servicer.API.Models
{
    public class UserTicket
    {
        public User User { get; set; }

        public int UserId { get; set; }

        public Ticket Ticket { get; set; }

        public int TicketId { get; set; }
    }
}
