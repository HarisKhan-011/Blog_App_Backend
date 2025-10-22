namespace Blog_App.Model
{
  public class CommentLike
  {
    public int Id { get; set; }
    public int CommentId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public Comment Comment { get; set; } = null!;
  }
}
