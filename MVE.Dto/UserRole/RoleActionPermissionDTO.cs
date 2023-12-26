using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVE.Dto
{
    public class RoleActionPermissionDTO
    {
        public long PageId { get; set; }
        public long RoleId { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
    }

    public class GeneralSiteSettingDTO
    {
        public string LogoImageName { get; set; } = null!;
        public string LogoImageNameDark { get; set; } = null!;
        public string SupportEmail { get; set; } = null!;
        public string SupportMobile { get; set; } = null!; 
        public int AdminPageLimit { get; set; }
    }
}
