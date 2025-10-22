using Blog_App.Data;
using Blog_App.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
namespace Blog_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  [Authorize]
  public class ProfileController : ControllerBase
    {
    private readonly AppDbContext _db;
    public ProfileController(AppDbContext db) => _db = db;

    [Authorize]
    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] Signup updatedProfile)
    {
      // Get the logged-in user's ID from JWT
      var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
      var user = await _db.Signups.FindAsync(int.Parse(userId));

      if (user == null)
        return NotFound(new { message = "User not found" });

      // Update allowed fields
      user.Name = updatedProfile.Name ?? user.Name;
      user.Email = updatedProfile.Email ?? user.Email;

      if (!string.IsNullOrEmpty(updatedProfile.Password))
      {
        var hasher = new PasswordHasher<Signup>();
        user.Password = hasher.HashPassword(user, updatedProfile.Password);
      }

      await _db.SaveChangesAsync();
      return Ok(new { message = "Profile updated successfully" });
    }

  }
}

