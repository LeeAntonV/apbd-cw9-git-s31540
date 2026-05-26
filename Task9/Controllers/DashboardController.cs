using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task9.Models;
using Task9.Services;
using Task9.ViewModels;

namespace Task9.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IUserRepository _repository;

    public DashboardController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        var notes = await _repository.GetNotesForUserAsync(userId);

        return View(new DashboardViewModel
        {
            Notes = notes
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddNote(DashboardViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Notes = await _repository.GetNotesForUserAsync(GetCurrentUserId());
            return View("Index", model);
        }

        var note = new UserNote
        {
            AppUserId = GetCurrentUserId(),
            Title = model.NewNote.Title.Trim(),
            Content = model.NewNote.Content.Trim()
        };

        await _repository.AddNoteAsync(note);
        return RedirectToAction(nameof(Index));
    }

    private Guid GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId)
            ? userId
            : throw new InvalidOperationException("Authenticated user is missing a valid id claim.");
    }
}
