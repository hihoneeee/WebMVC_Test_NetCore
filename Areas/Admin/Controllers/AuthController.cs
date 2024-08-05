using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using TestWebAPI.DTOs.Auth;
using TestWebAPI.Middlewares.Interfaces;
using TestWebAPI.Services.Interfaces;

namespace TestWebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICookieHelper _cookieHelper;

        public AuthController (IAuthService authService, IHttpContextAccessor httpContextAccessor, ICookieHelper cookieHelper)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _cookieHelper = cookieHelper;
        }

        public IActionResult Login()
        {
            return View(new AuthLoginDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Verify(AuthLoginDTO authLoginDTO)
        {
            var response = await _authService.LoginMvc(authLoginDTO);
            TempData["ToastMessage"] = response.message;
            TempData["ToastSuccess"] = response.success;

            if (response.success)
            {
                var roleCode = _cookieHelper.GetUserRole();
                if (roleCode == "D22MD2")
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else
                {
                    TempData["ToastMessage"] = "Invalid role.";
                    TempData["ToastSuccess"] = false;
                    return RedirectToAction("Login", "Auth", new { area = "Admin" });
                }
            }
            else
            {
                ModelState.AddModelError("", response.message);
                return RedirectToAction("Login", "Auth", new { area = "Admin" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth", new { area = "Admin" });
        }
    }
}
