using BBUAPI.Data;
using BBUAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace BBUAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<NewsController> _logger;

        public NewsController(AppDbContext context, ILogger<NewsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("ping")]
        public IActionResult Ping() => Ok(new { status = "working", time = DateTime.Now });


        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<News>>> GetNews()
        {
            var news = await _context.News
                .Include(n => n.Images)
                .ToListAsync();
            return news;
        }

        [HttpGet("details/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<News>> GetNewsById(int id)
        {
            var news = await _context.News
                .Include(n => n.Images)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (news == null)
                return NotFound();

            return news;
        }

        [HttpGet("getlatest/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<News>>> GetLatestNews(int id)
        {
            var news = await _context.News
                .Include(n => n.Images)
                .OrderByDescending(i => i.CreatedAt)
                .Take(id)
                .ToListAsync();
            return news;
        }

        [HttpPost]
        [Authorize(Policy = "ManageNews")]
        public async Task<IActionResult> CreateNews([FromForm] CreateNewsDto dto)
        {
            var news = new News
            {
                Title = dto.Title,
                Body = dto.Body
            };

            _context.News.Add(news);
            await _context.SaveChangesAsync();

            // Handle image uploads
            if (dto.Images != null && dto.Images.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "News");
                Directory.CreateDirectory(uploadsFolder);

                foreach (var image in dto.Images)
                {
                    if (image.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}_{image.FileName}";
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        // Save image metadata to database
                        var newsImage = new NewsImage
                        {
                            NewsId = news.Id,
                            FileName = fileName,
                            Url = $"/uploads/news/{fileName}"
                        };

                        _context.NewsImages.Add(newsImage);
                    }
                }

                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetNews), new { id = news.Id }, news);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Policy = "ManageNews")]
        public async Task<ActionResult<News>> DeleteNews(int id)
        {
            var news = await _context.News.FirstOrDefaultAsync(n => n.Id == id);
            if (news == null) return NotFound();

            _context.News.Remove(news);
            await _context.SaveChangesAsync();

            return news;
        }



    }
}
