using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mvc_app.Models;

namespace mvc_app.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View("Index");
    }
}
