namespace Blog_App.Model
{
  public class CommentReport
  {
    public int Id { get; set; }
    public int CommentId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; } = DateTime.UtcNow;
    public Comment Comment { get; set; } = null!;
  }
}
