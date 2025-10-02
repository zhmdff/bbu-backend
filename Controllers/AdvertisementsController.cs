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
    public class AdvertisementsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdvertisementsController> _logger;

        public AdvertisementsController(AppDbContext context, ILogger<AdvertisementsController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Advertisements>>> GetAdvertisements()
        {
            var advertisements = await _context.Advertisements.ToListAsync();
            var adsJson = JsonSerializer.Serialize(advertisements, new JsonSerializerOptions { WriteIndented = true });
            _logger.LogInformation("Advertisements: {AdsJson}", adsJson);
            return advertisements;
        }

        [HttpGet("details/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Advertisements>> GetAdvertisementsById(int id)
        {
            var advertisements = await _context.Advertisements
                .FirstOrDefaultAsync(n => n.Id == id);

            if (advertisements == null)
                return NotFound();

            return advertisements;
        }


        [HttpGet("getlatest/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Advertisements>>> GetLatestAdvertisements(int id)
        {
            var advertisements = await _context.Advertisements
                .OrderByDescending(i => i.CreatedAt)
                .Take(id)
                .ToListAsync();
            return advertisements;
        }


        [HttpPost]
        [Authorize(Policy = "ManageNews")]
        public async Task<IActionResult> CreateAdvertisements([FromForm] CreateAdvertisementDto dto)
        {
            var advertisements = new Advertisements
            {
                Title = dto.Title,
                Body = dto.Body
            };

            // Handle image upload
            if (dto.Image != null && dto.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Advertisements");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}_{dto.Image.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                advertisements.FileName = fileName;
                advertisements.Url = $"/uploads/advertisements/{fileName}";
            }

            _context.Advertisements.Add(advertisements);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAdvertisements), new { id = advertisements.Id }, advertisements);
        }


        [HttpDelete("delete/{id}")]
        [Authorize(Policy = "ManageNews")]
        public async Task<ActionResult<Advertisements>> DeleteAdvertisements(int id)
        {
            var advertisements = await _context.Advertisements.FirstOrDefaultAsync(n => n.Id == id);
            if (advertisements == null) return NotFound();

            _context.Advertisements.Remove(advertisements);
            await _context.SaveChangesAsync();

            return advertisements;
        }

    }
}
