using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System.Security.Claims;
using MVE.Core.Code.LIBS;
using MVE.Data.Models;
using MVE.Dto;

namespace MVE.Core.Models.Security
{
    public class CustomPrincipal
    {
        private readonly ClaimsPrincipal claimsPrincipal;


        public CustomPrincipal(ClaimsPrincipal principal)
        {

            claimsPrincipal = principal;
            IsAuthenticated = claimsPrincipal.Identity.IsAuthenticated;
            if (IsAuthenticated)
            {

                Id = Convert.ToInt32(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(Id))?.Value);
                FirstName = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(FirstName))?.Value;
                LastName = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(LastName))?.Value;
                IsActive = Convert.ToBoolean(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(IsActive))?.Value);
                Email = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Email)?.Value;
                Role = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Role)?.Value;
                _imageName = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(ImageName))?.Value;
                if (!string.IsNullOrEmpty(_imageName))
                   // _imageName = "/Uploads/Users/" + _imageName;
                    _imageName = SiteKeys.UploadFilesUsers + _imageName;
                else
                    _imageName = "/images/demo_user.jpg";
                FullName = $"{FirstName} {LastName}".Trim();
                if (claimsPrincipal.Claims.FirstOrDefault(u => u.Type == "userPermissions")?.Value != null)
                {
                    if (claimsPrincipal.Claims.FirstOrDefault(u => u.Type == "userPermissions")?.Value != null)
                        Permissions = JsonConvert.DeserializeObject<int[]>(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == "userPermissions")?.Value).ToArray();

                }

                if (claimsPrincipal.Claims.FirstOrDefault(u => u.Type == "allActionPermissions")?.Value != null)
                {
                    if (claimsPrincipal.Claims.FirstOrDefault(u => u.Type == "allActionPermissions")?.Value != null)
                        allActionPagePermissionList = JsonConvert.DeserializeObject<List<RoleActionPermissionDTO>>(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == "allActionPermissions")?.Value).ToList();

                }

                if (claimsPrincipal.Claims.FirstOrDefault(u => u.Type == "SiteSettings")?.Value != null)
                {
                    if (claimsPrincipal.Claims.FirstOrDefault(u => u.Type == "SiteSettings")?.Value != null)
                        SiteSettings = JsonConvert.DeserializeObject<GeneralSiteSettingDTO>(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == "SiteSettings")?.Value);

                }
            }
        }


        public bool IsAuthenticated { get; private set; }
        public int Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public bool IsActive { get; private set; }
        public string FullName { get; private set; }
        public string Email { get; private set; }

        public string Role { get; private set; }
        public int[] Roles { get; set; }
        public int[] Permissions { get; set; }
        public List<RoleActionPermissionDTO> allActionPagePermissionList { get; set; }
        private string _imageName;

        public string ImageName
        {
            get { return _imageName; }
            set
            {
                UpdateClaim(nameof(ImageName), value.ToString());
                _imageName = value;
            }
        }
        public bool IsInRole(Object roleType)
        {
            return Roles.Contains((int)roleType);
        }

        //public bool HasPermission(int[] permission)
        //{
        //    var hasPermission = false;
        //    bool isEmpty = Permissions.All(x => x == default(int));
        //    if (!isEmpty)
        //    {
        //         hasPermission= Permissions.Intersect(permission).Any();
        //    }
        //    return hasPermission;
        //}
        public bool HasPermission(int permissionId)
        {
            var hasPermission = false;
            //bool isEmpty = Permissions.All(x => x == default(int));
            //if (!isEmpty)
            //{
            hasPermission = Permissions.Contains(permissionId);
            //}
            return hasPermission;
        }

        private void UpdateClaim(string key, string value)
        {
            var claims = claimsPrincipal.Claims.ToList();
            if (claims.Any())
            {
                var pmClaim = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == key);
                if (pmClaim != null)
                {
                    claims.Remove(pmClaim);
                    claims.Add(new Claim(key, value));
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties { IsPersistent = true };
            ContextProvider.HttpContext.SignInAsync(
                  CookieAuthenticationDefaults.AuthenticationScheme,
                   new ClaimsPrincipal(claimsIdentity),
                   authProperties
                 ).Wait();
        }

        public GeneralSiteSettingDTO SiteSettings { get; set; }
    }
}
