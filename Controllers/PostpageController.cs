using Blog_App.Data;
using Blog_App.Model;
using Blog_App.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blog_App.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PostpageController : ControllerBase
  {
    private readonly AppDbContext _db;
    public PostpageController(AppDbContext db) => _db = db;

    // GET /api/postpage?page=1&pageSize=10&category=Education
    [Authorize(Roles = "Moderator,Admin")]
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? category = null)
    {
      var query = _db.Postpages.Include(p => p.Attachments).Include(p => p.comments).Include(p => p.Likes).AsQueryable();

      if (!string.IsNullOrEmpty(category))
        query = query.Where(p => p.PostCategory == category);

      var total = await query.CountAsync();
      var posts = await query.OrderByDescending(p => p.createdate)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync();

      return Ok(new { total, page, pageSize, posts });
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreatePost([FromBody] Postpage post)
    {
      var email = User.FindFirstValue(ClaimTypes.Email)
        ?? User.FindFirstValue(ClaimTypes.Name)
        ?? User.Identity?.Name ?? "";
      if (string.IsNullOrEmpty(email)) return Unauthorized("User email not found in token.");

      post.AuthorEmail = email;
      post.createdate = DateTime.UtcNow;
      post.Status = "Pending";

      post.IsApproved = false;
      post.IsPublished = false;
      post.IsDeleted = false;

      _db.Postpages.Add(post);
      await _db.SaveChangesAsync();
      return CreatedAtAction(nameof(Getpostbyid), new { id = post.Id }, post);
    }
    [HttpGet("approved")]
    public async Task<IActionResult> GetApprovedPosts(int page = 1, int pageSize = 4, string? category = null)
    {
      var query = _db.Postpages
          .Where(p => p.IsApproved && p.IsPublished && !p.IsDeleted);

      if (!string.IsNullOrEmpty(category) && category != "All")
        query = query.Where(p => p.PostCategory == category);

      var total = await query.CountAsync();
      var posts = await query
          .OrderByDescending(p => p.createdate)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();

      return Ok(new { posts, total });
    }



    [HttpPost("{postId}/comment")]
    [Authorize]
    public async Task<IActionResult> AddComment(int postId, [FromBody] AddCommentDto dto)
    {

      var post = await _db.Postpages
         .Include(p => p.comments)
         .FirstOrDefaultAsync(p => p.Id == postId);

      if (post == null)
        return NotFound("Post not found");

      if (string.IsNullOrWhiteSpace(dto.CommentText))
        return BadRequest("Comment text is required");

      var comment = new Comment
      {
        CommentText = dto.CommentText,
        PostId = postId,
        AuthorEmail = User.FindFirstValue(ClaimTypes.Email) ?? "",
        CommentDate = DateTime.UtcNow
      };

      _db.Comments.Add(comment);
      await _db.SaveChangesAsync();

      return Ok(new { message = "Comment added", comment });
    }


    [Authorize]
    [HttpPost("{id}/like")]
    public async Task<IActionResult> ToggleLike(int id)
    {

      //var userEmail = User.FindFirstValue(ClaimTypes.Name) ?? "";

      var userEmail = User.FindFirstValue(ClaimTypes.Email)
                ?? User.FindFirstValue(ClaimTypes.Name)
                ?? User.Identity?.Name;
      if (string.IsNullOrEmpty(userEmail))
        return Unauthorized("User email not found in token.");

      var existpost = await _db.Postpages.AnyAsync(p => p.Id == id);
      if (!existpost) return NotFound();

      var existing = await _db.PostLikes.FirstOrDefaultAsync(l => l.PostId == id && l.UserEmail == userEmail);

      if (existing != null)
      {
        _db.PostLikes.Remove(existing);
        await _db.SaveChangesAsync();
        var count = await _db.PostLikes.CountAsync(l => l.PostId == id);
        return Ok(new { liked = false, count });
      }
      else
      {
        var like = new PostLike { PostId = id, UserEmail = userEmail, CreatedAt = DateTime.UtcNow };
        _db.PostLikes.Add(like);
        await _db.SaveChangesAsync();
        var count = await _db.PostLikes.CountAsync(l => l.PostId == id);
        return Ok(new { liked = true, count });
      }
    }
    [HttpGet("details/{id}")]
    public async Task<IActionResult> GetRecentPosts(int id)
    {
      var posts = await _db.Postpages
          .Include(p => p.comments)
          .Include(p => p.Likes).Include(p => p.Attachments)
          .FirstOrDefaultAsync(p => p.Id == id);
      if (posts == null) return NotFound();
      return Ok(posts);
    }
    //[HttpPost("{postId}/comments")]
    //public async Task<IActionResult> Addcomment(int postId, [FromBody] Comment comments)
    //{
    //  var post = await _db.Postpages.FindAsync(postId);
    //  if (post == null) return NotFound();
    //  comments.PostId = postId;
    //  _db.Comments.Add(comments);
    //  await _db.SaveChangesAsync();

    //  return Ok(comments);
    //}

    [Authorize]
    [HttpPost("attachments/presign")]
    public IActionResult GetPresigned([FromBody] PresignRequest req)
    {
      // Use AWSSDK.S3 to create pre-signed URL
      // var s3 = new AmazonS3Client(awsKey, awsSecret, RegionEndpoint.USEast1);
      // var request = new GetPreSignedUrlRequest { BucketName = bucket, Key = $"posts/{Guid.NewGuid()}_{req.FileName}", Verb = HttpVerb.PUT, Expires = DateTime.UtcNow.AddMinutes(15) };
      // var url = s3.GetPreSignedURL(request);
      // return Ok(new { url, key = request.Key });

      // For brevity: return a fake response here; replace with real S3 code
      return Ok(new { url = "https://example.com/presigned-put-url", key = $"posts/{Guid.NewGuid()}_{req.FileName}" });
    }
    [HttpGet("single/{id}")]
    public async Task<IActionResult> Getpostbyid(int id)
    {
      var post = await _db.Postpages
          .Include(p => p.comments) // ✅ load comments
          .FirstOrDefaultAsync(p => p.Id == id);

      if (post == null)
        return NotFound();

      return Ok(post);
    }

    [Authorize]
    [HttpPost("{id}/report")]
    public async Task<IActionResult> ReportPost(int id, [FromBody] AddReportDto dto)
    {
      var post = await _db.Postpages.FindAsync(id);
      if (post == null) return NotFound("Post not found");

      var reporter = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.Name) ?? "";

      // optional: validate reason
      var allowedReasons = new[] { "Spam", "HateSpeech", "Violence", "FakeNews", "SexualContent", "Other" };
      if (string.IsNullOrWhiteSpace(dto.Reason) || !allowedReasons.Contains(dto.Reason))
        return BadRequest("Invalid reason");

      // check existing report (one per user per post)
      var exists = await _db.PostReports.FirstOrDefaultAsync(r => r.PostId == id && r.ReporterEmail == reporter);
      if (exists != null) return Conflict(new { message = "You already reported this post." });

      var report = new PostReport
      {
        PostId = id,
        ReporterEmail = reporter,
        Reason = dto.Reason,
        Details = dto.Details,
        CreatedAt = DateTime.UtcNow
      };

      _db.PostReports.Add(report);
      await _db.SaveChangesAsync();

      return Ok(new { message = "Report submitted" });
    }



  }
}
