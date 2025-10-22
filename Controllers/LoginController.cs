using Blog_App.Data;
using Blog_App.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Blog_App.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class LoginController : ControllerBase
  {
    private readonly AppDbContext _db;
    public LoginController(AppDbContext db)
    {
      _db = db;
    }
    [HttpPost ("login")]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
      try
      {
        var user = await _db.Signups.FirstOrDefaultAsync(u => u.Email == login.Email );

        if (user.LockoutEndTime.HasValue && user.LockoutEndTime > DateTime.UtcNow)
        {
          var remaining = user.LockoutEndTime.Value - DateTime.UtcNow;
          return Unauthorized(new { message = $"Account locked. Try again in {remaining.Minutes} minutes." });
        }

        if (user == null)
        {
          return Unauthorized(new { message = "Invalid email or password" });
        }
        //if (!user.IsEmailConfirmed)
        //{
        //  return Unauthorized(new { message = "Please confirm your email before logging in." });
        //}
        //hashing and salting
        var passwordhasher = new PasswordHasher<Signup>();
        var result = passwordhasher.VerifyHashedPassword(user, user.Password, login.Password);
        if (result == PasswordVerificationResult.Failed)
        {
          user.FailedLoginAttempts++;
          if (user.FailedLoginAttempts >= 5)
          {
            user.LockoutEndTime = DateTime.UtcNow.AddMinutes(5);
            user.FailedLoginAttempts = 0;
          }
          await _db.SaveChangesAsync();
          return Unauthorized(new { message = "invalid email and password" });
        }
        user.FailedLoginAttempts = 0;
        user.LockoutEndTime = null;
        await _db.SaveChangesAsync();
        // ✅ Generate JWT token
        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()??""),
        new Claim(JwtRegisteredClaimNames.Email,user.Email ?? ""),
         new Claim(ClaimTypes.Name, user.Name ?? ""),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Role,user.Role)
      };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsAReallyLongSuperSecretKey123456!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
        issuer: "https://localhost:5000",
        audience: "https://localhost:5000",
        claims: claims,
        expires: DateTime.Now.AddHours(1),
        signingCredentials: creds
    );
        var tokenstring = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new { token = tokenstring, message = "Login successful", user = user.Id , email = user.Email,
          name = user.Name,
          role = user.Role
        });
      }
      catch

        (Exception ex)
      {
        // log error so you can see in console
        Console.WriteLine("❌ Login error: " + ex.Message);
        return StatusCode(500, new { message = "Server error", error = ex.Message });
      }
    }
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
      // Only accessible with valid JWT
      return Ok("This is a protected profile data");
    }

  }
}
