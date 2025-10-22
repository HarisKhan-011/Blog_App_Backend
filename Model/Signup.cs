using System.ComponentModel.DataAnnotations.Schema;

namespace Blog_App.Model
{
  public class Signup
  {
    public  int Id { get; set; }
    public  string Name { get; set; }
    public  string Email { get; set; }
    public  string Password { get; set; }

    [NotMapped]
    public  string Conpassword { get; set; }
    public string Role { get; set; } = "User";
    public bool IsEmailConfirmed { get; set; } = false;
    public string? EmailConfirmationToken { get; set; }
    public DateTime? EmailConfirmationTokenExpiry { get; set; }

    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockoutEndTime { get; set; }

  }
}
