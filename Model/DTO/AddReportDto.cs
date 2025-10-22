namespace Blog_App.Model.DTO
{
  public class AddReportDto
  {
    public string Reason { get; set; } = string.Empty; // e.g. "Spam"
    public string? Details { get; set; }
  }
}
