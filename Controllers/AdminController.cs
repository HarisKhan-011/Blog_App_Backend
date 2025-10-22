using Blog_App.Data;
using Blog_App.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog_App.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(Roles = "Admin")]
  public class AdminController : ControllerBase
  {
    private readonly AppDbContext _db;
    public AdminController(AppDbContext db)
    {
      _db = db;
    }

    // 🧍 USERS MANAGEMENT
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
      var users = await _db.Signups
          .Select(u => new { u.Id, u.Name, u.Email, u.Role })
          .ToListAsync();
      return Ok(users);
    }

    [HttpPut("set-role/{userId}")]
    public async Task<IActionResult> SetUserRole(int userId, [FromBody] string role)
    {
      var user = await _db.Signups.FindAsync(userId);
      if (user == null) return NotFound("User not found");

      user.Role = role;
      await _db.SaveChangesAsync();
      return Ok(new { message = $"Role updated to {role}" });
    }

    [HttpDelete("delete-user/{userId}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
      var user = await _db.Signups.FindAsync(userId);
      if (user == null) return NotFound("User not found");

      _db.Signups.Remove(user);
      await _db.SaveChangesAsync();
      return Ok(new { message = "User deleted" });
    }

    // 📰 POSTS MANAGEMENT
    [HttpGet("posts")]
    public async Task<IActionResult> GetAllPosts()
    {
      var posts = await _db.Postpages
          .Select(p => new { p.Id, p.PostTitle,p.IsApproved, p.IsPublished, p.IsDeleted })
          .ToListAsync();
      return Ok(posts);
    }

    [HttpDelete("delete-post/{postId}")]
    public async Task<IActionResult> DeletePost(int postId)
    {
      var post = await _db.Postpages.FindAsync(postId);
      if (post == null) return NotFound("Post not found");

      _db.Postpages.Remove(post);
      await _db.SaveChangesAsync();
      return Ok(new { message = "Post deleted" });
    }

    // 💬 COMMENTS MANAGEMENT
    [HttpGet("comments")]
    public async Task<IActionResult> GetComments()
    {
      var comments = await _db.Comments.ToListAsync();
      return Ok(comments);
    }

    [HttpDelete("delete-comment/{commentId}")]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
      var comment = await _db.Comments.FindAsync(commentId);
      if (comment == null) return NotFound("Comment not found");

      _db.Comments.Remove(comment);
      await _db.SaveChangesAsync();
      return Ok(new { message = "Comment deleted" });
    }

    // 👍 LIKES MANAGEMENT
    // 👍 POST LIKES MANAGEMENT
    [HttpGet("post-likes")]
    public async Task<IActionResult> GetPostLikes()
    {
      var likes = await _db.PostLikes
          .Include(l => l.Post)
          .ToListAsync();
      return Ok(likes);
    }

    [HttpDelete("delete-post-like/{likeId}")]
    public async Task<IActionResult> DeletePostLike(int likeId)
    {
      var like = await _db.PostLikes.FindAsync(likeId);
      if (like == null) return NotFound("Post like not found");

      _db.PostLikes.Remove(like);
      await _db.SaveChangesAsync();
      return Ok(new { message = "Post like deleted" });
    }

    // 💬 COMMENT LIKES MANAGEMENT
    [HttpGet("comment-likes")]
    public async Task<IActionResult> GetCommentLikes()
    {
      var likes = await _db.CommentLikes
          .Include(l => l.Comment)
          .ToListAsync();
      return Ok(likes);
    }

    [HttpDelete("delete-comment-like/{likeId}")]
    public async Task<IActionResult> DeleteCommentLike(int likeId)
    {
      var like = await _db.CommentLikes.FindAsync(likeId);
      if (like == null) return NotFound("Comment like not found");

      _db.CommentLikes.Remove(like);
      await _db.SaveChangesAsync();
      return Ok(new { message = "Comment like deleted" });
    }


    // 🚨 REPORTS MANAGEMENT
    [HttpGet("reports")]
    public async Task<IActionResult> GetReports()
    {
      var reports = await _db.PostReports
          .Include(r => r.Post)
          .ToListAsync();
      return Ok(reports);
    }

    [HttpDelete("delete-report/{reportId}")]
    public async Task<IActionResult> DeleteReport(int reportId)
    {
      var report = await _db.PostReports.FindAsync(reportId);
      if (report == null) return NotFound("Report not found");

      _db.PostReports.Remove(report);
      await _db.SaveChangesAsync();
      return Ok(new { message = "Report deleted" });
    }

    // 💡 SUGGESTIONS MANAGEMENT
    [HttpGet("suggestions")]
    public async Task<IActionResult> GetSuggestions()
    {
      var suggestions = await _db.Suggestions.ToListAsync();
      return Ok(suggestions);
    }

    [HttpDelete("delete-suggestion/{suggestionId}")]
    public async Task<IActionResult> DeleteSuggestion(int suggestionId)
    {
      var suggestion = await _db.Suggestions.FindAsync(suggestionId);
      if (suggestion == null) return NotFound("Suggestion not found");

      _db.Suggestions.Remove(suggestion);
      await _db.SaveChangesAsync();
      return Ok(new { message = "Suggestion deleted" });
    }
  }
}
