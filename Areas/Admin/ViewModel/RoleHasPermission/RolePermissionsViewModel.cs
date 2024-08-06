using TestWebAPI.DTOs.Permisstion;
using TestWebAPI.DTOs.Role;

namespace TestWebMVC.Areas.Admin.ViewModel.RoleHasPermission
{
    public class RolePermissionsViewModel
    {
        public RoleDTO Role { get; set; }
        public List<PermisstionDTO> Permissions { get; set; }
        public List<string> SelectedPermissionIds { get; set; }

    }
}
