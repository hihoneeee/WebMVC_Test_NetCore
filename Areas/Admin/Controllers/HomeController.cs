using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TestWebAPI.Middlewares.Interfaces;
using TestWebMVC.Models;

namespace TestWebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICookieHelper _cookieHelper;
        public HomeController(ILogger<HomeController> logger, ICookieHelper cookieHelper) : base(cookieHelper)
        {
            _logger = logger;
            _cookieHelper = cookieHelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
