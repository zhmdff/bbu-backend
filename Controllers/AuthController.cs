using BBUAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private static readonly List<Admin> Admins = new()
    {
        new Admin { Username = "admin", Password = "admin123", Role = "SuperAdmin" },
        new Admin { Username = "zhmdff", Password = "mahmud123", Role = "Admin" },
        new Admin { Username = "vsadiq", Password = "sadiq123", Role = "Admin" },
        new Admin { Username = "moderator", Password = "mod123", Role = "Moderator" }
    };

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var admin = Admins.FirstOrDefault(a =>
            a.Username == request.Username && a.Password == request.Password);

        if (admin != null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("83bd79be213d67089df662cbe07c7f2c");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, admin.Username),
                    new Claim(ClaimTypes.Role, admin.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return Ok(new LoginResponse
            {
                Token = tokenString,
                Username = admin.Username
            });
        }
        return Unauthorized(new { message = "Invalid credentials" });
    }
}

public class Admin
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}