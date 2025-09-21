namespace BBUAPI.Models
{
    public class Advertisements
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CreateAdvertisementDto
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public IFormFile Image { get; set; }
    }

}
