﻿using Microsoft.AspNetCore.Mvc;
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
            var model = new GetCategoryDTO(); 
            return View("Form", model);
        }

        public async Task<IActionResult> Store(CategoryDTO categoryDTO)
        {
            var response = await _categoryServices.CreateCategoryAsync(categoryDTO);
            TempData["ToastMessage"] = response.message;
            TempData["ToastSuccess"] = response.success;
            if (response.statusCode == EHttpType.Success)
            {
                return RedirectToAction("Index");
            }
            else {
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var serviceResponse = await _categoryServices.GetCategoryByIdAsync(id);
            if (serviceResponse.data != null)
            {
                return View("Form", serviceResponse.data);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, CategoryDTO categoryDTO)
        {
            var serviceResponse = await _categoryServices.UpdateCategoryAsync(id, categoryDTO);
            TempData["ToastMessage"] = serviceResponse.message;
            TempData["ToastSuccess"] = serviceResponse.success;
            if (serviceResponse.statusCode == EHttpType.Success)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Edit");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var serviceResponse = await _categoryServices.DeleteCategoryAsync(id);
            TempData["ToastMessage"] = serviceResponse.message;
            TempData["ToastSuccess"] = serviceResponse.success;
            return RedirectToAction("Index");
        }
    }
}
