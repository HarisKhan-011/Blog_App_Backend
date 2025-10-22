namespace Blog_App.Model.DTO
{
  public class SuggestionDto
  {
    public string SuggestionText { get; set; } = string.Empty;
    public int PostId { get; set; }
    public int? ParentSuggestionId { get; set; }
  }
}
