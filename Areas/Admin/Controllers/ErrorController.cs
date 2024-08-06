using Microsoft.AspNetCore.Mvc;

namespace TestWebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ErrorController : Controller
    {
        public IActionResult NotFound()
        {
            return View();
        }
    }
}
