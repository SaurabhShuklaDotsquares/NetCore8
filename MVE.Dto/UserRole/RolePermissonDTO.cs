using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVE.Dto
{
    public class RolePermissonDTO
    {
        public long RolePageId { get; set; }
        public string PageName { get; set; }
        public string PageUrl { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; }


        public long RoleId { get; set; }
        public string RoleName { get; set; }    
        //public bool IsDeleted { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
    }
}
