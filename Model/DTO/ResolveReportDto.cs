namespace Blog_App.Model.DTO
{
  public class ResolveReportDto
  {
    public string? Action { get; set; } // "none" | "unpublish" | "delete"
    public string? Note { get; set; }
  }
}
