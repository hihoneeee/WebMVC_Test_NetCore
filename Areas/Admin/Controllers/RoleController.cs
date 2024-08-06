using Azure;
using Microsoft.AspNetCore.Mvc;
using TestWebAPI.DTOs.Role;
using TestWebAPI.DTOs.RoleHasPermission;
using TestWebAPI.Middlewares;
using TestWebAPI.Middlewares.Interfaces;
using TestWebAPI.Response;
using TestWebAPI.Services;
using TestWebAPI.Services.Interfaces;
using TestWebMVC.Areas.Admin.ViewModel.RoleHasPermission;
using static TestWebAPI.Response.HttpStatus;

namespace TestWebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly IPermissionServices _permissionServices;
        private readonly IRoleHasPermissionServices _roleHasPermissionServices;

        public RoleController(IRoleService roleService, IPermissionServices permissionServices, IRoleHasPermissionServices roleHasPermissionServices, ICookieHelper cookieHelper) : base(cookieHelper) { 
            _roleService = roleService;
            _permissionServices = permissionServices;
            _roleHasPermissionServices = roleHasPermissionServices;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _roleService.GetAllRoles();
            return View(response.data);
        }

        public async Task<IActionResult> Create()
        {
            var model = new RoleDTO();
            return View("Form", model);
        }

        public async Task<IActionResult> Store(AddRoleDTO addRoleDTO)
        {
            var response = await _roleService.AddRoleAsync(addRoleDTO);
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
            var response = await _roleService.GetRolesById(id);
            if (response.statusCode == EHttpType.Success)
            {
                return View("Form", response.data);
            }
            else
            {
                return RedirectToAction("NotFound", "Error");
            }
        }

        public async Task<IActionResult> Update(int id, AddRoleDTO addRoleDTO)
        {
            var response = await _roleService.UpdateRoleAsync(id, addRoleDTO);
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
            var response = await _roleService.DeleteRoleAsync(id);
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
        [HttpGet("assign-permissions/{id}")]
        public async Task<IActionResult> AssignPermissions(int id)
        {
            var roleResponse = await _roleService.GetRolesById(id);
            var permissionsResponse = await _permissionServices.GetAllPermissionAsyn();

            if (roleResponse.statusCode == EHttpType.Success && permissionsResponse.statusCode == EHttpType.Success)
            {
                var viewModel = new RolePermissionsViewModel
                {
                    Role = roleResponse.data,
                    Permissions = permissionsResponse.data,
                    SelectedPermissionIds = roleResponse.data.dataPermission.Select(p => p.value).ToList()
                };
                return View("AssignPermissions", viewModel);
            }
            else
            {
                return RedirectToAction("NotFound", "Error");
            }
        }

        [HttpPost("assign-permissions")]
        public async Task<IActionResult> AssignRolePermissions(AddRoleHasPermissionDTO addRoleHasPermissionDTO)
        {
            var response = await _roleHasPermissionServices.AssignPermissionsAsync(addRoleHasPermissionDTO);
            TempData["ToastMessage"] = response.message;
            TempData["ToastSuccess"] = response.success;
            if (response.statusCode == EHttpType.Success)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("AssignPermissions", new { id = addRoleHasPermissionDTO.codeRole });
            }
        }
    }
}
