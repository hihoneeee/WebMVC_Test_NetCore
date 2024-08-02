using Microsoft.AspNetCore.Mvc;
using TestWebAPI.Services;
using TestWebAPI.Services.Interfaces;

namespace TestWebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PropertyController : Controller
    {
        private readonly IPropertyServices _propertyServices;
        public PropertyController(IPropertyServices propertyServices)
        {
            _propertyServices = propertyServices;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
