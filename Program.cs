using TestWebAPI.Services.Interfaces;
using TestWebAPI.Services;
using TestWebAPI.Repositories.Interfaces;
using TestWebAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using TestWebAPI.Data;
using TestWebAPI.Helpers;
using AutoMapper;
using TestWebAPI.Settings;
using TestWebAPI.Middlewares;
using TestWebAPI.Helpers.IHelpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

// Config Jwt Bearer
var jwtSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<TokenSetting>(jwtSection);

var jwtSettings = jwtSection.Get<TokenSetting>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            RoleClaimType = "roleCode"
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var result = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    Success = "false",
                    message = "Token Invalid"
                });
                return context.Response.WriteAsync(result);
            }
        };
    });

// Builder settings
builder.Services.Configure<CloudinarySetting>(builder.Configuration.GetSection("CloudinarySetting"));
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddScoped<ICategoryServices, CategoryServices>();
builder.Services.AddScoped<ICloudinaryServices, CloudinaryServices>();
builder.Services.AddScoped<IAuthService, AuthServices>();
builder.Services.AddScoped<IJwtServices, JwtServices>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IRoleService, RoleServices>();
builder.Services.AddScoped<IPermissionServices, PermissionServices>();
builder.Services.AddScoped<ISendMailServices, SendMailServices>();
builder.Services.AddScoped<IRoleHasPermissionServices, RoleHasPermissionServices>();

// Add repositories to the container.
builder.Services.AddScoped<ICategoryRepositories, CategoryRepositories>();
builder.Services.AddScoped<IAuthRepositories, AuthRepositories>();
builder.Services.AddScoped<IJwtRepositories, JwtRepositories>();
builder.Services.AddScoped<IUserRepositories, UserRepositories>();
builder.Services.AddScoped<IPermisstionRepositories, PermisstionRepositories>();
builder.Services.AddScoped<IRoleHasPermissionRepositories, RoleHasPermissionRepositories>();
builder.Services.AddScoped<IRoleRepositories, RoleRepositories>();

// Add helpers to the container.
builder.Services.AddScoped<IJWTHelper, JWTHelper>();
builder.Services.AddScoped<IHashPasswordHelper, HashPasswordHelper>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

