using TestWebAPI.Services.Interfaces;
using TestWebAPI.Services;
using TestWebAPI.Repositories.Interfaces;
using TestWebAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using TestWebAPI.Data;
using TestWebAPI.Helpers;
using AutoMapper;
using TestWebAPI.Settings;
using TestWebAPI.Helpers.IHelpers;
using Microsoft.Extensions.Options;
using TestWebAPI.Configs;
using Microsoft.AspNetCore.Authentication.Cookies;
using TestWebAPI.Middlewares.Interfaces;
using TestWebAPI.Middlewares;
using TestWebMVC.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Connect DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("GoodRest"));
});

// AutoMapper
builder.Services.AddSingleton(provider => new MapperConfiguration(options =>
{
    options.AddProfile(new ApplicationMapper());
}).CreateMapper());

// AddSession
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Config Cookie Authentication 
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.LoginPath = "/admin/auth/login";
    options.AccessDeniedPath = "/admin/access-denied";
});

// Builder settings
builder.Services.Configure<CloudinarySetting>(builder.Configuration.GetSection("CloudinarySetting"));

// cookie
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddScoped<ICategoryServices, CategoryServices>();
builder.Services.AddScoped<ICloudinaryServices, CloudinaryServices>();
builder.Services.AddScoped<IAuthService, AuthServices>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IRoleService, RoleServices>();
builder.Services.AddScoped<IPermissionServices, PermissionServices>();
builder.Services.AddScoped<ISendMailServices, SendMailServices>();
builder.Services.AddScoped<IRoleHasPermissionServices, RoleHasPermissionServices>();
builder.Services.AddScoped<IJwtServices, JwtServices>();

// Add repositories to the container.
builder.Services.AddScoped<ICategoryRepositories, CategoryRepositories>();
builder.Services.AddScoped<IAuthRepositories, AuthRepositories>();
builder.Services.AddScoped<IUserRepositories, UserRepositories>();
builder.Services.AddScoped<IPermisstionRepositories, PermisstionRepositories>();
builder.Services.AddScoped<IRoleHasPermissionRepositories, RoleHasPermissionRepositories>();
builder.Services.AddScoped<IRoleRepositories, RoleRepositories>();
builder.Services.AddScoped<IJwtRepositories, JwtRepositories>();

// Add helpers to the container.
builder.Services.AddScoped<IHashPasswordHelper, HashPasswordHelper>();
builder.Services.AddSingleton<ICookieHelper, CookieHelper>();
builder.Services.AddScoped<IJWTHelper, JWTHelper>();

builder.Services.Configure<RedisCacheSetting>(builder.Configuration.GetSection("RedisCacheSetting"));
// Register the setting
builder.Services.AddSingleton<RedisCacheConfig>(provider =>
{
    var redisConfig = provider.GetRequiredService<IOptions<RedisCacheSetting>>().Value;
    return new RedisCacheConfig(redisConfig.ConnectionString);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CookieRoleMiddleware>();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "areas",
    pattern: "{controller=Error}/{action=NotFound}");
app.Run();