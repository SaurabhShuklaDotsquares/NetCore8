using Microsoft.AspNetCore.Mvc;
using MVE.Core;
using MVE.Service;
using MVE.Data.Models;
using MVE.Admin.ViewModels;
using MVE.DataTable.Extension;
using MVE.DataTable.Search;
using MVE.DataTable.Sort;
using MVE.DataTable.DataTables;
using MVE.Core.Code.Attributes;

namespace MVE.Admin.Controllers
{
    public class StaticController :  BaseController
    {
        private readonly IStaticService _staticService;

        public StaticController(IStaticService staticService)
        {
            this._staticService = staticService;

        }

        [CustomAuthorization(AppPermissions.Pages_Administration_StaticPage, AppPermissions.Action_IsRead)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(MVE.DataTable.DataTables.DataTable dataTable)
        {
            List<DataTableRow> table = new List<DataTableRow>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<StaticPage>();


            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.Trim().ToLower();
                query.AddFilter(q => q.Name.Contains(sSearch) || q.PageTitle.Contains(sSearch) ||q.MetaKeyword.Contains(sSearch) || q.MetaDescription.Contains(sSearch));
            }

            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<StaticPage, string>(q => q.Name, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<StaticPage, string>(q => q.Url, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<StaticPage, string>(q => q.PageTitle, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<StaticPage, string>(q => q.MetaKeyword, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 6:
                    query.AddSortCriteria(new ExpressionSortCriteria<StaticPage, string>(q => q.MetaDescription, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 7:
                    query.AddSortCriteria(new ExpressionSortCriteria<StaticPage, bool>(q => q.IsActive, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<StaticPage, DateTime>(q => q.AddedDate, SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;

            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<StaticPage> staticPages = _staticService.GetStaticPage(query, out total).Entities;
            foreach (StaticPage staticPage in staticPages)
            {
                // bool isRecipeExist = productService.IsProductExistForRecipe(product.ProductId);
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {

                    staticPage.StaticPageId.ToString(),
                    count.ToString(),
                    staticPage.Name,
                    staticPage.Url,
                    staticPage.PageTitle,
                    staticPage.MetaKeyword?.Length>35?$"{staticPage?.MetaKeyword?.Substring(0,35)}...":staticPage.MetaKeyword,
                    staticPage.MetaDescription?.Length>35?$"{staticPage?.MetaDescription?.Substring(0,35)}...":staticPage.MetaDescription,
                    staticPage.IsActive.ToString(),
                    staticPage.AddedDate.ToShortDateString()

                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }

        [CustomAuthorization(AppPermissions.Pages_Administration_StaticPage, AppPermissions.Action_IsCreate)]
        [HttpGet]
        public IActionResult CreateEdit(int? id)
        {
            StaticViewModel staticViewModel = new StaticViewModel();
            if (id.HasValue)
            {
                StaticPage staticPage = _staticService.GetStaticPageByPageId(id.Value);
                if (staticPage != null)
                {
                    staticViewModel.Name = staticPage.Name;
                    staticViewModel.PageTitle = staticPage.PageTitle;
                    staticViewModel.SelfUrl = staticPage.Url;
                    staticViewModel.MetaKeyword = staticPage.MetaKeyword;
                    staticViewModel.MetaDescription = staticPage.MetaDescription;
                    staticViewModel.Content = staticPage.Content;
                    staticViewModel.StaticPageId = staticPage.StaticPageId;
                }
            }
            return View(staticViewModel);
        }

        [HttpPost]
        public IActionResult CreateEdit(int? id, StaticViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    StaticPage staticPage = _staticService.GetStaticPageByPageId(id.Value);
                    if (staticPage != null)
                    {
                        staticPage.StaticPageId = model.StaticPageId;
                        staticPage.Content = model.Content;
                        staticPage.MetaDescription = model.MetaDescription;
                        staticPage.Name = model.Name;
                        staticPage.PageTitle = model.PageTitle;
                        staticPage.Url = model.SelfUrl;
                        staticPage.Ipaddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                        _staticService.Update(staticPage);
                        return NewtonSoftJsonResult(new RequestOutcome<dynamic> { RedirectUrl = Url.Action("index"),Message= "Page saved successfully.", IsSuccess = true });
                    }
                }
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = ex.Message, IsSuccess = false });

            }

            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Please ensure that all details should be filled", IsSuccess = false });

        }


        [CustomAuthorization(AppPermissions.Pages_Administration_StaticPage, AppPermissions.Action_IsCreate)]
        [HttpPost]
        public IActionResult ActiveStatus(int id)
        {
            StaticPage staticPage = _staticService.GetStaticPageByPageId(id);
            staticPage.IsActive = !staticPage.IsActive;
            _staticService.Update(staticPage);
            ////return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "Status Updated Successfully", IsSuccess = true });
            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = $"Status  {(staticPage.IsActive == true ? "Active" : "Inactive")} successfully.", IsSuccess = true });
        }
    }
}