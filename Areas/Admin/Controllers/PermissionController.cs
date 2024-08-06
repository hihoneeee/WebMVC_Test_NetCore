using Microsoft.AspNetCore.Mvc;
using TestWebAPI.DTOs.Permisstion;
using TestWebAPI.DTOs.Role;
using TestWebAPI.Services;
using TestWebAPI.Services.Interfaces;
using static TestWebAPI.Response.HttpStatus;

namespace TestWebMVC.Areas.Admin.Controllers
{
    public class PermissionController : Controller
    {
        private readonly IPermissionServices _permissionServices;

        public PermissionController(IPermissionServices permissionServices) {
            _permissionServices = permissionServices;
        }
        public async Task<IActionResult> Index()
        {
            var response = await _permissionServices.GetAllPermissionAsyn();
            return View(response);
        }
        public async Task<IActionResult> Create()
        {
            var model = new AddPermissionDTO();
            return View("Form", model);
        }

        public async Task<IActionResult> Store(AddPermissionDTO addPermissionDTO)
        {
            var response = await _permissionServices.CreatePermissionAsyn(addPermissionDTO);
            TempData["ToastMessage"] = response.message;
            TempData["ToastSuccess"] = response.success;
            if (response.statusCode == EHttpType.Success)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Create");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _permissionServices.GetPermissionByIdAsync(id);
            if (response.statusCode == EHttpType.Success)
            {
                return View("Form", response.data);
            }
            else
            {
                return RedirectToAction("NotFound", "Error");
            }
        }

        public async Task<IActionResult> Update(int id, AddPermissionDTO addPermissionDTO)
        {
            var response = await _permissionServices.UpdatePermissionAsyn(id, addPermissionDTO);
            TempData["ToastMessage"] = response.message;
            TempData["ToastSuccess"] = response.success;
            if (response.statusCode == EHttpType.Success)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Edit");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _permissionServices.DeletePermissionAsyn(id);
            TempData["ToastMessage"] = response.message;
            TempData["ToastSuccess"] = response.success;
            if (response.statusCode == EHttpType.Success)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("NotFound", "Error");
            }
        }
    }
}
