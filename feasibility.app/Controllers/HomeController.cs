using feasibility.Business.Middlewares;
using feasibility.DataAccess.Seeds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace feasibility.App.Controllers;


public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Index", "Dashboard");

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    [AllowAnonymous]
    [Route("Home/NotFound")]
    public new IActionResult NotFound() => View();
}
