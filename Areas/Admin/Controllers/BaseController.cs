using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TestWebAPI.Middlewares.Interfaces;

namespace TestWebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BaseController : Controller
    {
        protected readonly ICookieHelper _cookieHelper;

        public BaseController(ICookieHelper cookieHelper)
        {
            _cookieHelper = cookieHelper;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            SetUserInformation();
            base.OnActionExecuting(context);
        }

        private void SetUserInformation()
        {
            ViewBag.Avatar = _cookieHelper.GetUserAvatar();
            ViewBag.FullName = _cookieHelper.GetUserFullName();
            ViewBag.RoleName = _cookieHelper.GetUserRoleName();
        }
    }
}
