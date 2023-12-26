using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Core.Code.Attributes;
using MVE.Core.Code.LIBS;

namespace MVE.Core.Models.Security
{
    public class CustomPermission
    {
        [JsonProperty("id")]
        public readonly AppPermissions PermissionType;
        [JsonProperty("text")]
        public readonly string PermissionDisplayName;
        [JsonProperty("children")]
        public List<CustomPermission> ChildPermissions { get; set; }

        public CustomPermission CreateChildPermission(AppPermissions permissionName)
        {
            CustomPermission permission = new CustomPermission(permissionName);

            ChildPermissions.Add(permission);

            return permission;
        }

        public CustomPermission(AppPermissions permissionType)
        {
            PermissionType = permissionType;
            PermissionDisplayName = permissionType.GetEnumDescription();
            ChildPermissions = new List<CustomPermission>();
        }
    }
}
