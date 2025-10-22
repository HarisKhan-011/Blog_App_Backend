namespace Blog_App.Model
{
  public class PostReport
  {
    public int PostReportId { get; set; }
    public int PostId { get; set; }
    public Postpage? Post { get; set; }

    // Reporter identity
    public string ReporterEmail { get; set; } = string.Empty;

    // Predefined reason (Spam, HateSpeech, Violence, Other)
    public string Reason { get; set; } = string.Empty;

    public string? Details { get; set; } // optional free-text when "Other"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsResolved { get; set; } = false;
    public string? ResolvedBy { get; set; } 
    public DateTime? ResolvedAt { get; set; }
    public string? ResolutionNote { get; set; }
  }
}
