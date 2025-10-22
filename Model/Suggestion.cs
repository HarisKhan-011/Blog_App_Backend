namespace Blog_App.Model
{
  public class Suggestion
  {
    public int SuggestionId { get; set; }
    public string SuggestionText { get; set; } = string.Empty;
    public DateTime SuggestionDate { get; set; } = DateTime.Now;
    public int PostId { get; set; }
    public Postpage? Post { get; set; } = null!;
    public string AuthorEmail { get; set; } = string.Empty; // Who wrote suggestion
    public string Status { get; set; } = "Pending";
    // Possible values: Pending, Accepted, Rejected
    public int? ParentSuggestionId { get; set; } // For replies
    public Suggestion? ParentSuggestion { get; set; }
    public List<Suggestion> Replies { get; set; } = new();
  }
}
