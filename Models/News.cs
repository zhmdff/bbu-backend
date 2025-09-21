using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace BBUAPI.Models
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public List<NewsImage> Images { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class NewsImage
    {
        public int Id { get; set; }
        public int NewsId { get; set; }

        [JsonIgnore]
        public News News { get; set; }

        // store only metadata in DB
        public string FileName { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CreateNewsDto
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public IFormFile[] Images { get; set; }
    }

}
