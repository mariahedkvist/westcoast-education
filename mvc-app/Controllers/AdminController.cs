using Microsoft.AspNetCore.Mvc;

namespace mvc_app.Controllers;

[Route("admin")]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View("Index");
    }
}