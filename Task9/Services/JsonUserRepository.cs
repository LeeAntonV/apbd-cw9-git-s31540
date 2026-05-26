using System.Text.Json;
using Task9.Models;

namespace Task9.Services;

public class JsonUserRepository : IUserRepository
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public JsonUserRepository(IWebHostEnvironment environment)
    {
        var dataDirectory = Path.Combine(environment.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dataDirectory);
        _filePath = Path.Combine(dataDirectory, "users.json");
    }

    public async Task<AppUser?> FindByEmailAsync(string email)
    {
        var normalizedEmail = NormalizeEmail(email);
        var store = await ReadStoreAsync();
        return store.Users.FirstOrDefault(user => NormalizeEmail(user.Email) == normalizedEmail);
    }

    public async Task<AppUser?> FindByIdAsync(Guid id)
    {
        var store = await ReadStoreAsync();
        return store.Users.FirstOrDefault(user => user.Id == id);
    }

    public async Task<IReadOnlyList<AppUser>> GetUsersAsync()
    {
        var store = await ReadStoreAsync();
        return store.Users.OrderBy(user => user.Email).ToList();
    }

    public async Task<IReadOnlyList<UserNote>> GetNotesForUserAsync(Guid appUserId)
    {
        var store = await ReadStoreAsync();
        return store.Notes
            .Where(note => note.AppUserId == appUserId)
            .OrderByDescending(note => note.CreatedAt)
            .ToList();
    }

    public async Task AddUserAsync(AppUser user)
    {
        await _lock.WaitAsync();
        try
        {
            var store = await ReadStoreUnlockedAsync();

            if (store.Users.Any(existing => NormalizeEmail(existing.Email) == NormalizeEmail(user.Email)))
            {
                throw new InvalidOperationException("A user with this email already exists.");
            }

            store.Users.Add(user);
            await WriteStoreUnlockedAsync(store);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task AddNoteAsync(UserNote note)
    {
        await _lock.WaitAsync();
        try
        {
            var store = await ReadStoreUnlockedAsync();
            store.Notes.Add(note);
            await WriteStoreUnlockedAsync(store);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<UserStore> ReadStoreAsync()
    {
        await _lock.WaitAsync();
        try
        {
            return await ReadStoreUnlockedAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<UserStore> ReadStoreUnlockedAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new UserStore();
        }

        await using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<UserStore>(stream, SerializerOptions) ?? new UserStore();
    }

    private async Task WriteStoreUnlockedAsync(UserStore store)
    {
        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, store, SerializerOptions);
    }

    private static string NormalizeEmail(string email) => email.Trim().ToUpperInvariant();

    private sealed class UserStore
    {
        public List<AppUser> Users { get; set; } = [];
        public List<UserNote> Notes { get; set; } = [];
    }
}
