namespace Task9.Models;

public class AppUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = UserRoles.User;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
