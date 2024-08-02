using Microsoft.AspNetCore.Mvc;
using TestWebAPI.DTOs.Auth;
using TestWebAPI.Services.Interfaces;
using static TestWebAPI.Response.HttpStatus;

namespace TestWebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController (IAuthService authService)
        {
            _authService = authService;
        }
        public IActionResult Login()
        {
            return View(new AuthLoginDTO());
        }
        public async Task<IActionResult> Verify(AuthLoginDTO authLoginDTO)
        {
            var response = await _authService.Login(authLoginDTO);
            TempData["ToastMessage"] = response.message;
            TempData["ToastSuccess"] = response.success;
            if (response.statusCode == EHttpType.Success)
            {
                HttpContext.Session.SetString("access_token", response.accessToken);
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else
            {
                ModelState.AddModelError("", response.message);
                return RedirectToAction("Login", "Auth", new { area = "Admin" });
            }
        }

    }
}
