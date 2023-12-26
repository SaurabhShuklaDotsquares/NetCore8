using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Core.Code.Attributes;
using MVE.Core.Models.Security;

namespace MVE.Core.Models
{
    public class AppPermissionTree
    {
        public static readonly CustomPermission Permissions = null;
        private AppPermissionTree() { }

        static AppPermissionTree()
        {
            Permissions = new CustomPermission(AppPermissions.Pages);

            // Administration Users
            //CustomPermission administrationUsers = Permissions.CreateChildPermission(AppPermissions.Pages_Administration_AdminUsers);
            //administrationUsers.CreateChildPermission(AppPermissions.Pages_Administration_AdminUsers_List);
            //administrationUsers.CreateChildPermission(AppPermissions.Pages_Administration_AdminUsers_Create);
            //administrationUsers.CreateChildPermission(AppPermissions.Pages_Administration_AdminUsers_Edit);
            //administrationUsers.CreateChildPermission(AppPermissions.Pages_Administration_AdminUsers_Delete);

            //// Roles
            //CustomPermission rolePermissions = Permissions.CreateChildPermission(AppPermissions.Pages_Administration_Roles);
            //rolePermissions.CreateChildPermission(AppPermissions.Pages_Administration_Roles_List);
            //rolePermissions.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Create);
            //rolePermissions.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Edit);
            //rolePermissions.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Delete);
        }
    }
}
