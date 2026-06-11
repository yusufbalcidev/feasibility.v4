using Microsoft.AspNetCore.Mvc;

namespace feasibility.App.Controllers;

public class TarifeFizibiliteController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
