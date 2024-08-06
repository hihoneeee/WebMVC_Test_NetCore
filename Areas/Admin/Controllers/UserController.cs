using Microsoft.AspNetCore.Mvc;
using TestWebAPI.Middlewares.Interfaces;
using TestWebAPI.Services.Interfaces;

namespace TestWebMVC.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserServices _userServices;
        public UserController(ICookieHelper cookieHelper, IUserServices userServices) : base(cookieHelper)
        {
            _userServices = userServices;
        }
        public async Task<IActionResult> Index()
        {
            var response = await _userServices.GetUserInSystemAsync();
            return View(response.data);
        }
    }
}
