namespace servicer.API.Dtos
{
    public class NoteForSendDto
    {
        public string Text { get; set; }

        public int? ProductSpecificationId { get; set; }

        public int? ItemId { get; set; }

        public int? CustomerId { get; set; }
    }
}