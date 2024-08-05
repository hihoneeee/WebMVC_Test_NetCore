using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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

            // Bỏ qua kiểm tra cho các trang đăng nhập
            if (path.StartsWithSegments("/admin/auth/login") || path.StartsWithSegments("/admin/auth/verify"))
            {
                await _next(context);
                return;
            }

            if (path.StartsWithSegments("/admin"))
            {
                var roleCode = _cookieHelper.GetUserRole();

                if (string.IsNullOrEmpty(roleCode) || roleCode != "D22MD2")
                {
                    context.Response.Redirect("/admin/auth/login");
                    return;
                }
            }

            await _next(context);
        }
    }
}
