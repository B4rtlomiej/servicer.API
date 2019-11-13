namespace servicer.API.Dtos
{
    public class ProductSpecificationForListDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Manufacturer { get; set; }

        public string Series { get; set; }

        public bool IsActive { get; set; }
    }
}