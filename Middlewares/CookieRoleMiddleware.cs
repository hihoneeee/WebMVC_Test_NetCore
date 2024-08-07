using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using TestWebAPI.Middlewares.Interfaces;

namespace TestWebMVC.Middlewares
{
    public class CookieRoleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICookieHelper _cookieHelper;

        public CookieRoleMiddleware(RequestDelegate next, ICookieHelper cookieHelper)
        {
            _next = next;
            _cookieHelper = cookieHelper;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;

            // Bỏ qua kiểm tra cho các trang xác minh
            if (path.StartsWithSegments("/admin/auth/verify"))
            {
                await _next(context);
                return;
            }

            var roleCode = _cookieHelper.GetUserRole();
            var roleName = _cookieHelper.GetUserRoleName();

            // Kiểm tra nếu người dùng đã có cookie và đang truy cập trang đăng nhập
            if (!string.IsNullOrEmpty(roleCode) && path.StartsWithSegments("/admin/auth/login"))
            {
                context.Response.Redirect("/admin");
                return;
            }

            // Kiểm tra quyền truy cập các trang admin
            if (path.StartsWithSegments("/admin") && !path.StartsWithSegments("/admin/auth"))
            {
                if (string.IsNullOrEmpty(roleCode) || roleName == "Buyer" && roleName == "Seller")
                {
                    context.Response.Redirect("/admin/auth/login");
                    return;
                }
            }

            await _next(context);
        }
    }
}
