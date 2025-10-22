namespace Blog_App.Model
{
  public class PostLike
  {
    public int Id { get; set; }
    public int PostId { get; set; }
    public Postpage Post { get; set; } = null!;
    public string UserEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}
