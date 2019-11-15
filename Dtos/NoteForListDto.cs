namespace servicer.API.Dtos
{
    public class NoteForListDto
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public int? ProductSpecificationId { get; set; }

        public int? ItemId { get; set; }

        public int? CustomerId { get; set; }
    }
}