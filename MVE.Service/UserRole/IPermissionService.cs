using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;

namespace MVE.Service
{
    public interface IPermissionService
    {
        int[] GetPermissionByRoleId(long roleId);
        List<MenuItem> GetAllMenus();
        bool DeletePermission(List<UserPermission> userPermissions);
        void InsertPermissionList(List<UserPermission> permissions);
    }
}
