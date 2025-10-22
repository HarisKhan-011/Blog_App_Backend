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
  public class ModerationController : ControllerBase
  {
    private readonly AppDbContext _db;
    public ModerationController(AppDbContext db) => _db = db;

    // 1. Get posts pending approval (not approved and not deleted)
    [Authorize(Policy = "ModeratorOnly")]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingPosts(int page = 1, int pageSize = 20)
    {
      var q = _db.Postpages
          .Where(p => !p.IsApproved && !p.IsDeleted)
          .OrderByDescending(p => p.createdate);

      var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
      return Ok(items);
    }

    // 2. Get reported posts (unresolved reports)
    [Authorize(Policy = "ModeratorOnly")]
    [HttpGet("reported")]
    public async Task<IActionResult> GetReportedPosts(int page = 1, int pageSize = 20)
    {
      var q = _db.PostReports
          .Where(r => !r.IsResolved)
          .Include(r => r.Post)
          .OrderByDescending(r => r.CreatedAt);

      var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
      return Ok(items);
    }

    // 3. Approve post (mark approved and publish)
    [Authorize(Policy = "ModeratorOnly")]
    [HttpPut("approve/{postId}")]
    public async Task<IActionResult> ApprovePost(int postId)
    {
      var post = await _db.Postpages.FindAsync(postId);
      if (post == null) return NotFound("Post not found");

      post.IsApproved = true;
      post.IsPublished = true;
      await _db.SaveChangesAsync();
      return Ok(new { message = "Post approved" });
    }

    // 4. Unpublish or mark unpublished (owner or moderator can unpublish)
    [Authorize(Policy = "ModeratorOnly")]
    [HttpPut("unpublish/{postId}")]
    public async Task<IActionResult> UnpublishPost(int postId)
    {
      var post = await _db.Postpages.FindAsync(postId);
      if (post == null) return NotFound("Post not found");

      post.IsPublished = false;
      await _db.SaveChangesAsync();
      return Ok(new { message = "Post unpublished" });
    }

    // 5. Soft-delete post
    [Authorize(Policy = "ModeratorOnly")]
    [HttpDelete("delete/{postId}")]
    public async Task<IActionResult> DeletePost(int postId)
    {
      var post = await _db.Postpages.FindAsync(postId);
      if (post == null) return NotFound("Post not found");

      post.IsDeleted = true;
      await _db.SaveChangesAsync();
      return Ok(new { message = "Post soft-deleted" });
    }

    // 6. Resolve a report (mark resolved + optional action)
    [Authorize(Policy = "ModeratorOnly")]
    [HttpPut("report/resolve/{reportId}")]
    public async Task<IActionResult> ResolveReport(int reportId, [FromBody] ResolveReportDto dto)
    {
      var report = await _db.PostReports.Include(r => r.Post).FirstOrDefaultAsync(r => r.PostReportId == reportId);
      if (report == null) return NotFound("Report not found");

      report.IsResolved = true;
      report.ResolvedBy = User.FindFirst(ClaimTypes.Email)?.Value;
      report.ResolvedAt = DateTime.UtcNow;
      report.ResolutionNote = dto.Note;

      if (dto.Action == "unpublish")
      {
        if (report.Post != null)
        {
          report.Post.IsPublished = false;
        }
      }
      else if (dto.Action == "delete")
      {
        if (report.Post != null) report.Post.IsDeleted = true;
      }

      await _db.SaveChangesAsync();
      return Ok(new { message = "Report resolved" });
    }
  }
}
