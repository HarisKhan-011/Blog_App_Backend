using Microsoft.EntityFrameworkCore;

namespace Blog_App.Model
{
  public class Postpage
  {
    public int Id { get; set; }
    public string PostTitle { get; set; } = string.Empty;
    public string PostContent { get; set; }= string.Empty;
    public string PostCategory { get; set; }= string.Empty;

    public DateTime createdate { get; set; } = DateTime.UtcNow;

    public string AuthorEmail { get; set; } = string.Empty; // store who created it

    public string Status { get; set; } = "Pending";
    public List<PostLike> Likes { get; set; } = new();

    public List<Comment> comments { get; set; } = new();
    public List<PostAttachment> Attachments { get; set; } = new();

    public List<Suggestion> Suggestions { get; set; } = new();

    public bool IsApproved { get; set; } = false;   
    public bool IsPublished { get; set; } = true;  
    public bool IsDeleted { get; set; } = false;

  }
}
