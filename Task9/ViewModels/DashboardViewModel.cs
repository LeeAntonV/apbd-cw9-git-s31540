using Task9.Models;

namespace Task9.ViewModels;

public class DashboardViewModel
{
    public CreateNoteViewModel NewNote { get; set; } = new();
    public IReadOnlyList<UserNote> Notes { get; set; } = [];
}
