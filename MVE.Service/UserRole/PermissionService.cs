using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.Repo;

namespace MVE.Service
{
    public class PermissionService : IPermissionService
    {
        #region "Fields"

        private readonly IRepository<UserPermission> _repoPermission;
        private readonly IRepository<MenuItem> _repoMenuItem;

        #endregion

        #region "Constructor"

        public PermissionService(IRepository<UserPermission> repoPermission, IRepository<MenuItem> repoMenuItem)
        {
            _repoPermission = repoPermission;
            _repoMenuItem = repoMenuItem;
        }

        #endregion

        #region "Methods"

        public UserPermission GetPermission(long Id)
        {
            return _repoPermission.FindById(Id);
        }

        public bool DeletePermission(List<UserPermission> model)
        {
            foreach (var item in model)
            {
                UserPermission entity = GetPermission(item.Id);
                _repoPermission.Delete(entity);
            }
            _repoPermission.SaveChanges();
            return true;
        }

        public void InsertPermissionList(List<UserPermission> permissionList)
        {
            foreach (var per in permissionList)
            {
                _repoPermission.InsertGraph(per);
            }
        }
        public int[] GetPermissionByRoleId(long roleId)
        {
            return _repoPermission.Query().Filter(x => x.RoleId.Equals(roleId)).Get().Select(x => x.UserPermissionId).ToArray();
        }

        public List<MenuItem> GetAllMenus()
        {
            return _repoMenuItem.Query().Filter(x => x.PermissionId.Value != 100 && x.IsActive == true).Get().OrderBy(x => x.OrderIndex).ToList();
        }
        #endregion "Methods"

        #region "Dispose"
        public void Dispose()
        {
            if (_repoPermission != null)
            {
                _repoPermission.Dispose();
            }
        }
        #endregion
    }
}
