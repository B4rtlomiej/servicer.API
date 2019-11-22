using servicer.API.Models;

namespace servicer.API.Helpers
{
    public class TicketParams
    {
        private const int MaxPageSize = 50;

        public int PageNumber { get; set; } = 1;

        private int pageSize = 10;
        
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }
        public Priority Priority { get; set; }

        public Status Status { get; set; }

        public string OrderBy { get; set; } =  "lastOpen";

        public string Token { get; set; }
    }
}