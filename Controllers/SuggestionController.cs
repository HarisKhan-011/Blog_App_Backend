using Blog_App.Data;
using Blog_App.Model;
using Blog_App.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blog_App.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class SuggestionController : ControllerBase
  {
    private readonly AppDbContext _db;
    public SuggestionController(AppDbContext db) => _db = db;

    // ✅ Add Suggestion
    [Authorize]
    [HttpPost("add")]
    public async Task<IActionResult> AddSuggestion([FromBody] Suggestion model)
    {
      try
      {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        if (userEmail == null)
          return BadRequest("User email not found. Are you logged in?");

        model.AuthorEmail = userEmail;
        model.SuggestionDate = DateTime.Now;

        await _db.Suggestions.AddAsync(model);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Suggestion added successfully!" });
      }
      catch (Exception ex)
      {
        return StatusCode(500, ex.ToString()); // 🔥 Send real error back
      }
    }

    // ✅ Reply to Suggestion
    [Authorize]
    [HttpPost("reply/{parentId}")]
    public async Task<IActionResult> ReplyToSuggestion(int parentId, SuggestionDto dto)
    {
      var parent = await _db.Suggestions.FindAsync(parentId);
      if (parent == null) return NotFound("Parent suggestion not found");

      var reply = new Suggestion
      {
        SuggestionText = dto.SuggestionText,
        PostId = parent.PostId,
        ParentSuggestionId = parentId,
        AuthorEmail = User.FindFirst(ClaimTypes.Email)?.Value,
        SuggestionDate = DateTime.Now,
        Status = "Pending"
      };

      _db.Suggestions.Add(reply);
      await _db.SaveChangesAsync();

      return Ok(new { message = "Reply added successfully", reply });
    }

    // ✅ Get Suggestions by Post
    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetSuggestionsByPost(int postId)
    {
      var suggestions = await _db.Suggestions
          .Where(s => s.PostId == postId && s.ParentSuggestionId == null)
          .Include(s => s.Replies)
          .OrderByDescending(s => s.SuggestionDate)
          .ToListAsync();

      return Ok(suggestions);
    }

    // ✅ Approve Suggestion
    [Authorize]
    [HttpPut("approve/{id}")]
    public async Task<IActionResult> ApproveSuggestion(int id)
    {
      var suggestion = await _db.Suggestions.FindAsync(id);
      if (suggestion == null) return NotFound("Suggestion not found");

      suggestion.Status = "Accepted";
      await _db.SaveChangesAsync();

      return Ok(new { message = "Suggestion approved!" });
    }

    // ✅ Reject Suggestion
    [Authorize]
    [HttpPut("reject/{id}")]
    public async Task<IActionResult> RejectSuggestion(int id)
    {
      var suggestion = await _db.Suggestions.FindAsync(id);
      if (suggestion == null) return NotFound("Suggestion not found");

      suggestion.Status = "Rejected";
      await _db.SaveChangesAsync();

      return Ok(new { message = "Suggestion rejected!" });
    }

    // ✅ Get Suggestions by User
    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMySuggestions()
    {
      var email = User.FindFirst(ClaimTypes.Email)?.Value;
      var mySuggestions = await _db.Suggestions
          .Where(s => s.AuthorEmail == email)
          .OrderByDescending(s => s.SuggestionDate)
          .ToListAsync();

      return Ok(mySuggestions);
    }

    // ✅ Delete Suggestion
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSuggestion(int id)
    {
      var suggestion = await _db.Suggestions.FindAsync(id);
      if (suggestion == null) return NotFound("Suggestion not found");

      _db.Suggestions.Remove(suggestion);
      await _db.SaveChangesAsync();
      return Ok(new { message = "Suggestion deleted!" });
    }
  
  [Authorize]
    [HttpGet("received")]
    public async Task<IActionResult> GetReceivedSuggestions()
    {
      var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
      if (userEmail == null) return Unauthorized();

      var postIds = await _db.Postpages
          .Where(p => p.AuthorEmail == userEmail)
          .Select(p => p.Id)
          .ToListAsync();

      var suggestions = await _db.Suggestions
          .Where(s => postIds.Contains(s.PostId) && s.ParentSuggestionId == null)
          .Include(s => s.Replies)
          .OrderByDescending(s => s.SuggestionDate)
          .ToListAsync();

      return Ok(suggestions);
    }
  } 
  }
