using Microsoft.AspNetCore.Mvc;

namespace Task9.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return RedirectToAction("Login", "Account");
    }

    public IActionResult Error()
    {
        return View();
    }
}
