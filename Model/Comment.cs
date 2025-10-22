namespace Blog_App.Model
{
  public class Comment
  {
    public int CommentId { get; set; }
    public string CommentText { get; set; } = string.Empty;
    public DateTime CommentDate { get; set; } = DateTime.Now;
    public int PostId { get; set; }
    public string AuthorEmail { get; set; } = string.Empty;
    public Postpage Post { get; set; } = null!;

    public int? ParentCommentId { get; set; } // For replies
    public Comment? ParentComment { get; set; }
    public List<Comment> Replies { get; set; } = new(); // Child comments

    public List<CommentLike> Likes { get; set; } = new(); // Likes per comment
    public List<CommentReport> Reports { get; set; } = new(); // Reports per comment

  }

}
