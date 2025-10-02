using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,SuperAdmin,Moderator")]
public class AdminController : ControllerBase
{
    [HttpGet("dashboard")]
    public IActionResult GetDashboard()
    {
        return Ok(new { message = $"Welcome to admin dashboard, {User.Identity?.Name}", user = User.Identity?.Name });
    }

    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        return Ok(new[] {
            new { id = 1, name = "Mahmud Market", email = "zhmdff@gmail.com" },
            new { id = 2, name = "Sadick Online", email = "sadick@gmail.com" },
            new { id = 3, name = "Olayı Bilenler", email = "olayi@bilenler.com" }
        });
    }
}