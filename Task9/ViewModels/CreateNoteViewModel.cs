using System.ComponentModel.DataAnnotations;

namespace Task9.ViewModels;

public class CreateNoteViewModel
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;
}
