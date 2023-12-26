using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.Design;
using MVE.Core.Code.LIBS;
using MVE.Core.Models.Security;
using MVE.Data.Models;
using MVE.DataTable.Extension;
using MVE.DataTable.Search;
using MVE.DataTable.Sort;
namespace MVE.Admin.ViewModels
{
    public class FrontUserViewModel
    {
        public FrontUserViewModel() { }
        #region [Public Properties]
        public long Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        public string Email { get; set; } = null!;

        public string EncryptedPassword { get; set; } = null!;

        public string SaltKey { get; set; } = null!;

        [Required(ErrorMessage = "First name is required")]
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public byte? Gender { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string? Address { get; set; }

        public string? City { get; set; }

        public string? StateOrCounty { get; set; }

        public int? CountryId { get; set; }

        public string? ZipCode { get; set; }

        [Required(ErrorMessage = "Phone no is required")]
        public string? MobilePhone { get; set; }

        public string? ForgotPasswordLink { get; set; }

        public DateTime? ForgotPasswordLinkExpired { get; set; }

        public bool? ForgotPasswordLinkUsed { get; set; }

        public string ImageName { get; set; } = null!;

        public bool IsEmailVerified { get; set; }

        public string? EmailOtp { get; set; }

        public DateTime? EmailVerificationOtpExpired { get; set; }

        public string? EmailVerificationToken { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationOn { get; set; }

        public long CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public long? ModifiedBy { get; set; }
        public int? TotalBookings { get; set; }
        [Required(ErrorMessage = "New password is required")]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*\\W).{8,16}|null$", ErrorMessage = "Password must contains atleast 1 Uppercase , 1 Lowercase ,1 Numeric Character, 1 Special Character and length should be 8 to 16 Characters")]
        public string NewPassword { get; set; }

        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Confirm password doesn't match, Type again")]
        public string ConfirmPassword { get; set; }
        public bool IsPassUpdate { get; set; }
        #endregion [Public Properties]

        private MVE.DataTable.DataTables.DataTable DataTablesRequest { get; set; }
        public FrontUserViewModel(MVE.DataTable.DataTables.DataTable dataTablesRequest)
        {
            DataTablesRequest = dataTablesRequest;
        }
        public bool isComposed { get; set; }
        public List<DataTableRow> GridViewData { get; set; }
        private CustomPrincipal CurrentUser { get; set; }
        private IWebHostEnvironment WebHostEnvironment { get; set; }
        private User _FrontUser { get; set; }
        private IEnumerable<User> _FrontUserList { get; set; }
        public List<SelectListItem> roleDropDownList { get; set; } = new List<SelectListItem>();
        public SearchQuery<User> FilterGrid(bool? status, int sortIndex, StringValues sortDirection)
        {
            var query = new SearchQuery<User>();

            if (!string.IsNullOrEmpty(DataTablesRequest.sSearch))
            {
                string sSearch = DataTablesRequest.sSearch.ToLower().Trim().TrimStart().TrimEnd();

                query.AddFilter(ad => (ad.FirstName + " " + ad.LastName).Contains(sSearch) ||
                 ad.Email.ToLower().Contains(sSearch.Trim()) ||
                 ad.Id.ToString().Contains(sSearch) ||
                 ad.MobilePhone.ToLower().Contains(sSearch) || ad.Address.ToLower().Contains(sSearch)
                 );
            }

            query.AddFilter(ad => ad.IsDeleted == false);
            if (status != null)
                query.AddFilter(b => b.IsActive == status);


            query.Take = DataTablesRequest.iDisplayLength;
            query.Skip = DataTablesRequest.iDisplayStart;

            return ShortGrid(query, sortIndex, sortDirection);
        }
        private SearchQuery<User> ShortGrid(SearchQuery<User> query, int sortIndex, StringValues sortDirection)
        {
            switch (sortIndex)
            {
                case 1:
                    query.AddSortCriteria(new ExpressionSortCriteria<User, long>(q => q.Id, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<User, string>(q => q.FirstName + " " + q.LastName, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
              
                case 7:
                     query.AddSortCriteria(new ExpressionSortCriteria<User, DateTime>(q => q.CreationOn, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<User, DateTime>(q => q.CreationOn, SortDirection.Descending));
                    break;
            }
            return query;
        }
        public void SetEntity(IEnumerable<User> frontUsers)
        {
            _FrontUserList = frontUsers;
        }
        public void ComposeViewData()
        {
            if (_FrontUserList == null)
            {
                BindFrontUserViewData();
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

            foreach (var item in _FrontUserList)
            {
                GridViewData.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    count.ToString(),
                     item.Id.ToString(),
                    (!string.IsNullOrEmpty(item.FirstName) ? " " + item.FirstName : item.FirstName) + " " +(!string.IsNullOrEmpty(item.LastName) ? " " + item.LastName : item.LastName),
                   item.ImageName,
                   item.Email,
                   item.MobilePhone,
                   item.Address,
                   item.CreationOn.ToString(SiteKeys.DateFormatWithoutTime),
                    item.IsActive==true?"Active":"InActive"
                });
                count++;
            }
        }

        private void BindFrontUserViewData()
        {
            throw new NotImplementedException();
        }
    }
}
