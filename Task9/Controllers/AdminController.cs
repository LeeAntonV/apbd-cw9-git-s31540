using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task9.Models;
using Task9.Services;

namespace Task9.Controllers;

[Authorize(Roles = UserRoles.Admin)]
public class AdminController : Controller
{
    private readonly IUserRepository _repository;

    public AdminController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _repository.GetUsersAsync();
        return View(users);
    }
}
