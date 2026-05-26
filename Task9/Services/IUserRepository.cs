using Task9.Models;

namespace Task9.Services;

public interface IUserRepository
{
    Task<AppUser?> FindByEmailAsync(string email);
    Task<AppUser?> FindByIdAsync(Guid id);
    Task<IReadOnlyList<AppUser>> GetUsersAsync();
    Task<IReadOnlyList<UserNote>> GetNotesForUserAsync(Guid appUserId);
    Task AddUserAsync(AppUser user);
    Task AddNoteAsync(UserNote note);
}
