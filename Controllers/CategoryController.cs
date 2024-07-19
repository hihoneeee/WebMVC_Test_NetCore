using Microsoft.AspNetCore.Mvc;
using TestWebAPI.DTOs.Category;
using TestWebAPI.Response;
using TestWebAPI.Services.Interfaces;
using static TestWebAPI.Response.HttpStatus;

namespace TestWebMVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryServices _categoryServices;
        public CategoryController(ICategoryServices categoryServices)
        {
            _categoryServices = categoryServices;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var serviceResponse = await _categoryServices.GetCategoriesAsync();
            if (serviceResponse.statusCode == EHttpType.Success)
            {
                return View(serviceResponse.data);
            }
            else
            {
                return StatusCode((int)serviceResponse.statusCode, new { serviceResponse.success, serviceResponse.message });
            }
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var serviceResponse = await _categoryServices.GetCategoryByIdAsync(id);
            if (serviceResponse.data != null)
            {
                return View(serviceResponse.data);
            }
            return View("Error", new { message = serviceResponse.message });
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryDTO categoryDTO)
        {
            var serviceResponse = await _categoryServices.UpdateCategoryAsync(id, categoryDTO);
            if (serviceResponse.statusCode == EHttpType.Success)
            {
                ViewData["ToastMessage"] = serviceResponse.message;
                return RedirectToAction("Index");
            }
            else
            {
                return StatusCode((int)serviceResponse.statusCode, new { serviceResponse.success, serviceResponse.message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var serviceResponse = await _categoryServices.DeleteCategoryAsync(id);
            if (serviceResponse.statusCode == EHttpType.Success)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return StatusCode((int)serviceResponse.statusCode, new { serviceResponse.success, serviceResponse.message });
            }
        }
    }
}
