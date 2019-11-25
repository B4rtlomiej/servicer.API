using servicer.API.Models;

namespace servicer.API.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }
        public bool IsActive { get; set; } = true;
        public UserRole UserRole { get; set; }
        public string OrderBy { get; set; } = "lastActive";
    }
}