namespace Blog_App.Model.DTO
{
  public class AddCommentRequest
  {
    public string CommentText { get; set; }
    public int PostId { get; set; }
  }
}
