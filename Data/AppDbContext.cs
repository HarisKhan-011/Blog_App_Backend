using Blog_App.Model;
using Microsoft.EntityFrameworkCore;

namespace Blog_App.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options)
      :base(options)
    { }
    

    public DbSet<Postpage> Postpages { get; set; }
    public DbSet<Signup> Signups { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<PostLike> PostLikes { get; set; }
    public DbSet<PostAttachment> PostAttachments { get; set; }
    public DbSet<PostReport> PostReports { get; set; }
    public DbSet<CommentLike> CommentLikes { get; set; }
    public DbSet<CommentReport> CommentReports { get; set; }

    public DbSet<Suggestion> Suggestions { get; set; }

  }
}
