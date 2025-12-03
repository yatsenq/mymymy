namespace StayFit.UI.Services;

public interface ICurrentUserService
{
    int? UserId { get; set; }
    string? Email { get; set; }
    string? FirstName { get; set; }
}

public class CurrentUserService : ICurrentUserService
{
    public int? UserId { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
}
