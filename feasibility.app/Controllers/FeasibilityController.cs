using feasibility.Business.Middlewares;
using feasibility.DataAccess.Seeds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace feasibility.App.Controllers;
[PagePermission(PageSeed.FeasibilityKey)]

public class FeasibilityController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
