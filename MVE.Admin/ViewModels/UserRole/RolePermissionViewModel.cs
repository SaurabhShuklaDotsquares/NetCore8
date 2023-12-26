using MVE.Core.Models.Security;
using MVE.Data.Models;
using MVE.Dto;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace MVE.Admin.ViewModels
{
    public class RolePermissionViewModel
    {
        public RolePermissionViewModel() { }
        private CustomPrincipal CurrentUser { get; set; }

        private IHostingEnvironment _HostingEnvironment { get; set; }

        #region [Public Properties]

        public long RoleId { get; set; }
        public string RoleName { get; set; }
        #endregion [Public Properties]

        #region [Common Properties]
        private IEnumerable<RolePage> _RolePageList { get; set; }
        private UserRole _UserRole { get; set; }
        public List<RolePermissonDTO> RolePermissionList { get; set; }

        public bool isComposed { get; set; }

        #endregion [Common Properties]

        #region [Binding Methods]
        public void SetEntity(CustomPrincipal currentUser)
        {
            CurrentUser = currentUser;
        }
        public void SetHostingEnvironment(IHostingEnvironment hostingEnvironment)
        {
            _HostingEnvironment = hostingEnvironment;
        }
        public void SetEntity(IEnumerable<RolePage> entity)
        {
            _RolePageList = entity;
        }
        public void SetEntityRole(UserRole roleEntity)
        {
            _UserRole = roleEntity;
        }
      
        public void ComposeViewData()
        {
            if (_RolePageList != null)
            {
                BindRolePermissionList(_UserRole);
            }
            isComposed = true;
        }

        private void BindRolePermissionList(UserRole _UserRole)
        {
            RolePermissionList = new List<RolePermissonDTO>();
            foreach (var item in _RolePageList)
            {
                RolePermissonDTO rolePermisson = new RolePermissonDTO();
                rolePermisson.RolePageId = item.Id;
                rolePermisson.PageName = item.PageName;
                rolePermisson.PageUrl = item.PageUrl;
                rolePermisson.OrderIndex = item.OrderIndex;
                rolePermisson.IsActive = item.IsActive;
                rolePermisson.RoleId = _UserRole.Id;
                rolePermisson.RoleName = _UserRole.RoleName;
                rolePermisson.IsReadOnly = item.RolePagePermissions.Where(x => x.PageId == item.Id && x.RoleId == _UserRole.Id).ToList().Count > 0 ? item.RolePagePermissions.FirstOrDefault(x => x.PageId == item.Id && x.RoleId == _UserRole.Id).IsReadOnly : false;
                rolePermisson.IsCreate = item.RolePagePermissions.Where(x => x.PageId == item.Id && x.RoleId == _UserRole.Id).ToList().Count > 0 ? item.RolePagePermissions.FirstOrDefault(x => x.PageId == item.Id && x.RoleId == _UserRole.Id).IsCreate : false;
                rolePermisson.IsEdit = item.RolePagePermissions.Where(x => x.PageId == item.Id && x.RoleId == _UserRole.Id).ToList().Count > 0 ? item.RolePagePermissions.FirstOrDefault(x => x.PageId == item.Id && x.RoleId == _UserRole.Id).IsEdit : false;
                rolePermisson.IsDelete = item.RolePagePermissions.Where(x => x.PageId == item.Id && x.RoleId == _UserRole.Id).ToList().Count > 0 ? item.RolePagePermissions.FirstOrDefault(x => x.PageId == item.Id && x.RoleId == _UserRole.Id).IsDelete : false;

                RolePermissionList.Add(rolePermisson);
            }
        }




        #endregion [Binding Methods]
    }
}
