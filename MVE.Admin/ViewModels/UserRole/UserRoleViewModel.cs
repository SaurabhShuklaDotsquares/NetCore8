using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVE.Admin.ViewModels
{
    public class UserRoleViewModel
    {
        public UserRoleViewModel()
        {
            RolePermissionList = new List<PermissionViewModel>();
        }

        public string Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "*required")]
        [Remote("IsRoleNameExist", "UserRole", HttpMethod = "POST", AdditionalFields = "PreInitialRoleName", ErrorMessage = "Role Name already exists")]
        public string Name { get; set; }

        [DisplayName("Status")]
        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string PreInitialRoleName { get; set; }
        public string PermissionSelectedIds { get; set; }
        public List<PermissionViewModel> RolePermissionList { get; set; }

        public string RoleType { get; set; }

    }

    public class PermissionViewModel
    {
        public long Id { get; set; }

        ///PermissionId is a EnumId in AppPermissions class
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime CreationTime { get; set; }
        public int CreatorUserId { get; set; }
        public string CreatorUserName { get; set; }

    }
}
