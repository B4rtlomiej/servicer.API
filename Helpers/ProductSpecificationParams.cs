namespace servicer.API.Helpers
{
    public class ProductSpecificationParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }
        public string IsActive { get; set; } = "active";
        public string Column { get; set; }
        public string Sorting { get; set; }
    }
}