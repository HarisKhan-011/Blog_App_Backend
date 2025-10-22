namespace Blog_App.Model
{
  public class PostAttachment
  {
    public int Id { get; set; }
    public int PostId { get; set; }
    public Postpage Post { get; set; } = null!;
    public string Key { get; set; } = string.Empty;    // S3 key
    public string Url { get; set; } = string.Empty;    // public URL or proxy URL
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}
