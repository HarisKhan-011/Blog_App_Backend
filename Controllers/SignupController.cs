using Blog_App.Data;
using Blog_App.Model;
using Blog_App.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blog_App.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SignupController : ControllerBase
    {
    private readonly AppDbContext _db;
    public SignupController(AppDbContext db) => _db=db;

    [HttpPost]
    public async Task<IActionResult> Signupdataadd([FromBody] Signup signup)
    {
      if (signup.Role == "Admin" || signup.Role == "Moderator")
      {
        signup.Role = "User";
      }
      if (await _db.Signups.AnyAsync(u => u.Email == signup.Email))
        return BadRequest(new { message = "Email already resgistered." });

      signup.Role = "User";
      var passwordhasher = new PasswordHasher<Signup>();
      signup.Password = passwordhasher.HashPassword(signup, signup.Password);

      signup.EmailConfirmationToken = Guid.NewGuid().ToString();
      signup.EmailConfirmationTokenExpiry = DateTime.UtcNow.AddHours(24);
      signup.IsEmailConfirmed = false;

      _db.Signups.Add(signup);
      await _db.SaveChangesAsync();
      var confirmurl = $"https://localhost:5000/api/signup/confirm-email?token={signup.EmailConfirmationToken}";
      Console.WriteLine($"✅ Email confirmation link: {confirmurl}");

      //return CreatedAtAction(nameof(getSignup), new { id = signup.SignupId }, signup);
      return Ok(new { message = "User registered successfully. Please check email to confirm." });

    }
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string token)
    {
      var user = await _db.Signups.FirstOrDefaultAsync(u => u.EmailConfirmationToken == token);
      if (user == null)
        return BadRequest("Invalid or expired token.");

      if (user.EmailConfirmationTokenExpiry.HasValue && user.EmailConfirmationTokenExpiry < DateTime.UtcNow)
        return BadRequest("Token expired. Request a new confirmation email.");

      user.IsEmailConfirmed = true;
      user.EmailConfirmationToken = null;
      user.EmailConfirmationTokenExpiry = null;
      await _db.SaveChangesAsync();
      return Ok("✅ Email confirmed successfully!");
    }
    [Authorize(Roles = "Admin")]
    [HttpPatch("set-role/{userId}")]
    public async Task<IActionResult> SetRole(int userId, [FromBody] UpdateRoleDto dto)
    {
      if (dto == null || string.IsNullOrWhiteSpace(dto.NewRole))
        return BadRequest("Role is required.");

      var normalizedRole = dto.NewRole.Trim();

      // Only allow these roles
      var allowedRoles = new[] { "User", "Moderator", "Admin" };
      if (!allowedRoles.Contains(normalizedRole))
        return BadRequest("Invalid role. Allowed: User, Moderator, Admin");

      var user = await _db.Signups.FindAsync(userId);
      if (user == null) return NotFound("User not found");

      // Prevent admin from changing their own role (safety)
      var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
      if (!string.IsNullOrEmpty(currentUserEmail) && string.Equals(currentUserEmail, user.Email, StringComparison.OrdinalIgnoreCase))
      {
        return BadRequest("You cannot change your own role.");
      }

      user.Role = normalizedRole;
      await _db.SaveChangesAsync();

      return Ok(new { message = $"Role updated to {normalizedRole}", userId, newRole = normalizedRole });
    }
    [HttpPost("change-email")]
    public async Task<IActionResult> ChangeEmail(string oldEmail,string newEmail)
    {
      var user = await _db.Signups.FirstOrDefaultAsync(u => u.Email == oldEmail);
      if (user == null) return NotFound("User not found");

      user.Email = newEmail;
      user.IsEmailConfirmed = false;
      user.EmailConfirmationToken = Guid.NewGuid().ToString();
      user.EmailConfirmationTokenExpiry = DateTime.UtcNow.AddHours(1);
      await _db.SaveChangesAsync();
      return Ok("Confirmation email sent to new address.");

    }

    

    [Authorize(Roles = "Admin")]
    [HttpGet("all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
      var users = await _db.Signups.ToListAsync();
      return Ok(users);
    }

    [HttpGet]
    public async  Task<IActionResult> getSignup(int id)
    {
      var getvalue = await _db.Signups.FindAsync(id);
      if (getvalue == null) return NotFound();
      return Ok(getvalue);
    }
  }
}
