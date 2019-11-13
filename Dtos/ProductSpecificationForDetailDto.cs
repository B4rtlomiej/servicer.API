namespace servicer.API.Dtos
{
    public class ProductSpecificationForDetailDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Manufacturer { get; set; }

        public string Series { get; set; }

        public bool IsActive { get; set; }
    }
}