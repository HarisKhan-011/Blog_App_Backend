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
  [Authorize] // Require login for all actions
  public class CommentController : ControllerBase
  {
    private readonly AppDbContext _db;

    public CommentController(AppDbContext db)
    {
      _db = db;
    }

    // ✅ Get comments of a post with pagination
    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetComments(int postId, int page = 1, int pageSize = 5)
    {
      var comments = await _db.Comments
          .Where(c => c.PostId == postId && c.ParentCommentId == null)
          .Include(c => c.Replies)
          .Include(c => c.Likes)
          .Include(c => c.Reports)
          .OrderByDescending(c => c.CommentDate)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();

      var totalComments = await _db.Comments.CountAsync(c => c.PostId == postId && c.ParentCommentId == null);

      return Ok(new
      {
        TotalComments = totalComments,
        Page = page,
        PageSize = pageSize,
        Data = comments
      });
    }

    // ✅ Add a comment
    [HttpPost]
    public async Task<IActionResult> AddComment(Comment comment)
    {
      comment.AuthorEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "anonymous";

      _db.Comments.Add(comment);
      await _db.SaveChangesAsync();

      return Ok(new { message = "Comment added successfully", comment });
    }

    // ✅ Reply to comment
    [HttpPost("ReplyToComment/{parentId}")]
    [Authorize]
    public async Task<IActionResult> ReplyToComment(int parentId, AddCommentRequest request)
    {
      var parent = await _db.Comments.FindAsync(parentId);
      if (parent == null) return NotFound("Parent comment not found");

      var reply = new Comment
      {
        CommentText = request.CommentText,
        CommentDate = DateTime.Now,
        PostId = request.PostId,
        ParentCommentId = parentId,
        AuthorEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "anonymous"
      };
      _db.Comments.Add(reply);
      await _db.SaveChangesAsync();
      return Ok(new { message = "Reply added successfully", reply });
    }

    // ✅ Like a comment (1 like per user)
    [HttpPost("like/{commentId}")]
    public async Task<IActionResult> ToggleCommentLike(int commentId)
    {
      var email = User.FindFirst(ClaimTypes.Email)?.Value;
      if (email == null)
        return Unauthorized("User not logged in");

      var existingLike = await _db.CommentLikes
          .FirstOrDefaultAsync(l => l.CommentId == commentId && l.UserEmail == email);

      if (existingLike != null)
      {
        // ✅ Unlike
        _db.CommentLikes.Remove(existingLike);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Unliked comment", liked = false });
      }
      else
      {
        // ✅ Like
        var newLike = new CommentLike
        {
          CommentId = commentId,
          UserEmail = email
        };
        _db.CommentLikes.Add(newLike);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Liked comment", liked = true });
      }
    }

      // ✅ Unlike a comment
      [HttpDelete("unlike/{commentId}")]
    public async Task<IActionResult> UnlikeComment(int commentId)
    {
      var email = User.FindFirst(ClaimTypes.Email)?.Value;
      var like = await _db.CommentLikes
          .FirstOrDefaultAsync(l => l.CommentId == commentId && l.UserEmail == email);

      if (like == null) return NotFound("You did not like this comment");

      _db.CommentLikes.Remove(like);
      await _db.SaveChangesAsync();

      return Ok(new { message = "Comment unliked" });
    }

    // ✅ Report a comment
    [HttpPost("report/{commentId}")]
    public async Task<IActionResult> ReportComment(int commentId, [FromBody] string reason)
    {
      var email = User.FindFirst(ClaimTypes.Email)?.Value;

      _db.CommentReports.Add(new CommentReport
      {
        CommentId = commentId,
        UserEmail = email!,
        Reason = reason
      });

      await _db.SaveChangesAsync();

      return Ok(new { message = "Comment reported" });
    }
  }
}
