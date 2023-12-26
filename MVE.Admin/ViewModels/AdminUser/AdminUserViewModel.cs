using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using MVE.Core.Code.LIBS;
using MVE.Core.Models.Security;
using MVE.Data.Models;
using MVE.DataTable.Extension;
using MVE.DataTable.Search;
using MVE.DataTable.Sort;

namespace MVE.Admin.ViewModels
{
    public class AdminUserViewModel
    {
        public AdminUserViewModel() { }

        #region [Public Properties]
        public long Id { get; set; }

        public long RoleId { get; set; }

        public string Email { get; set; }

        public string EncryptedPassword { get; set; }

        public string SaltKey { get; set; }

        public byte Title { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public byte Gender { get; set; }

        public string? Address { get; set; }

        public string? MobilePhone { get; set; }

        public string? ForgotPasswordLink { get; set; }

        public DateTime? ForgotPasswordLinkExpired { get; set; }

        public bool ForgotPasswordLinkUsed { get; set; }

        public string? ImageName { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public long? ModifiedBy { get; set; }
        #endregion [Public Properties]

        private MVE.DataTable.DataTables.DataTable DataTablesRequest { get; set; }
        public AdminUserViewModel(MVE.DataTable.DataTables.DataTable dataTablesRequest)
        {
            DataTablesRequest = dataTablesRequest;
        }
        public bool isComposed { get; set; }
        public List<DataTableRow> GridViewData { get; set; }
        private CustomPrincipal CurrentUser { get; set; }
        private IWebHostEnvironment WebHostEnvironment { get; set; }
        private AdminUser _AdminUser { get; set; }
        private IEnumerable<AdminUser> _AdminUserList { get; set; }
        public List<SelectListItem> roleDropDownList { get; set; } = new List<SelectListItem>();
        public SearchQuery<AdminUser> FilterGrid(bool? status, int sortIndex, StringValues sortDirection)
        {
            var query = new SearchQuery<AdminUser>();

            if (!string.IsNullOrEmpty(DataTablesRequest.sSearch))
            {
                string sSearch = DataTablesRequest.sSearch.ToLower().Trim();

                query.AddFilter(ad => (ad.FirstName + " " + ad.LastName).Contains(sSearch) ||
                 ad.Email.ToLower().Contains(sSearch.Trim()) ||
                 ad.FirstName.ToLower().Contains(sSearch) ||
                 ad.LastName.ToLower().Contains(sSearch)
                 );
            }

            query.AddFilter(ad => ad.IsDeleted == false);
            if (status != null)
                query.AddFilter(b => b.IsActive == status);


            query.Take = DataTablesRequest.iDisplayLength;
            query.Skip = DataTablesRequest.iDisplayStart;

            return ShortGrid(query, sortIndex, sortDirection);
        }
        private SearchQuery<AdminUser> ShortGrid(SearchQuery<AdminUser> query, int sortIndex, StringValues sortDirection)
        {
            switch (sortIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<AdminUser, string>(q => q.FirstName + " " + q.LastName, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<AdminUser, string>(q => q.Role.RoleName, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<AdminUser, bool>(q => (bool)q.IsActive, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 6:
                    query.AddSortCriteria(new ExpressionSortCriteria<AdminUser, DateTime>(q => q.CreationOn, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 7:
                    query.AddSortCriteria(new ExpressionSortCriteria<AdminUser, DateTime>(q => q.ModifiedOn, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<AdminUser, DateTime>(q => q.CreationOn, SortDirection.Descending));
                    break;
            }
            return query;
        }
        public void SetEntity(IEnumerable<AdminUser> adminUsers)
        {
            _AdminUserList = adminUsers;
        }
        public void ComposeViewData()
        {
            if (_AdminUserList == null)
            {
                BindAdminUserViewData();
            }
            else
            {
                BindGridViewData();
            }
            isComposed = true;
        }

        private void BindGridViewData()
        {
            GridViewData = new List<DataTableRow>();
            int count = DataTablesRequest.iDisplayStart + 1;

            foreach (var item in _AdminUserList)
            {
                GridViewData.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    item.Id.ToString(),
                    count.ToString(),
                    (!string.IsNullOrEmpty(item.FirstName) ? " " + item.FirstName : item.FirstName) + " " +(!string.IsNullOrEmpty(item.LastName) ? " " + item.LastName : item.LastName),
                    //GetCompanyNameForGrid(item.CompanyId),
                    item.Role.RoleName,
                    item.Email,
                    item.MobilePhone,
                    item.Address,
                    item.CreationOn.ToString(SiteKeys.DateFormatWithoutTime),
                    //item.ModifiedOn.ToString(SiteKeys.DateFormatWithoutTime),
                    Convert.ToString(item.IsActive),
                    item.RoleId.ToString()
                });
                count++;
            }
        }

        private void BindAdminUserViewData()
        {
            throw new NotImplementedException();
        }
    }

    public class AdminUserManageViewModel
    {
        public long Id { get; set; }

        public long RoleId { get; set; }

        [Required(ErrorMessage = "Email required")]
        public string Email { get; set; }

        public string EncryptedPassword { get; set; }

        public string SaltKey { get; set; }

        public byte Title { get; set; }
        [Required(ErrorMessage = "First name required")]
        public string FName { get; set; }
        [Required(ErrorMessage = "Last name required")]
        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public byte Gender { get; set; }
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string? Address { get; set; }
        public bool IsPassUpdate { get; set; }
        public string? Description { get; set; }

        public string? MobilePhone { get; set; }

        public string? ImageName { get; set; }
        [Required(ErrorMessage = "Password required.")]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*\\W).{8,16}$", ErrorMessage = "Password must contains atleast 1 Uppercase , 1 Lowercase ,1 Numeric Character, 1 Special Character and length should be 8 to 16 Characters")]
        public string Password { get; set; }
        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Confirm password required.")]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again")]
        public string ConfirmPassword { get; set; }
        public List<SelectListItem> roleDropDownList { get; set; } = new List<SelectListItem>();
        public string? TypeMode { get; set; }
    }
}
