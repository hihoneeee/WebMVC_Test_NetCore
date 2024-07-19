using TestWebAPI.Services.Interfaces;
using TestWebAPI.Services;
using TestWebAPI.Repositories.Interfaces;
using TestWebAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using TestWebAPI.Data;
using TestWebAPI.Helpers;
using AutoMapper;
using TestWebAPI.Settings;

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

//builder settings
builder.Services.Configure<CloudinarySetting>(builder.Configuration.GetSection("CloudinarySetting"));

// Add services to the container.
builder.Services.AddScoped<ICategoryServices, CategoryServices>();
builder.Services.AddScoped<ICloudinaryServices, CloudinaryServices>();

// Add repositories to the container.
builder.Services.AddScoped<ICategoryRepositories, CategoryRepositories>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
