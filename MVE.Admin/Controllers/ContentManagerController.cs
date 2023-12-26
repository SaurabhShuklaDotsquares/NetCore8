using Microsoft.AspNetCore.Mvc;
using MVE.Admin.ViewModels;
using MVE.Core;
using MVE.Core.Code.Attributes;
using MVE.Core.Code.LIBS;
using MVE.Data.Models;
using MVE.DataTable.Extension;
using MVE.DataTable.Search;
using MVE.DataTable.Sort;
using MVE.Service;
using MVE.Service.ContentManager;

namespace MVE.Admin.Controllers
{
    public class ContentManagerController : BaseController
    {
        private readonly IContentManagerService _contentManagerService;
        public ContentManagerController(IContentManagerService contentManagerService)
        {
            _contentManagerService = contentManagerService;
        }
        #region [Index]

        [CustomAuthorization(AppPermissions.Pages_Administration_StaticPage, AppPermissions.Action_IsRead)]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(MVE.DataTable.DataTables.DataTable dataTable, string? dstatus)
        {

            List<DataTableRow> table = new List<DataTableRow>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<StaticPage>();
            query.AddFilter(q => q.IsActive == true);
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.Trim().ToLower();
                query.AddFilter(q => (q.Name ?? "").Contains(sSearch) || q.PageTitle.Contains(sSearch) || (q.Url ?? "").Contains(sSearch));
            }

            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<StaticPage, string>(q => q.Name ?? "", sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<StaticPage, string>(q => q.PageTitle, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<StaticPage, DateTime>(q => q.AddedDate, SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;

            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<StaticPage> obj = _contentManagerService.Get(query, out total).Entities;

            foreach (StaticPage r in obj)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    r.StaticPageId.ToString(),
                    count.ToString(),
                    r.Name??string.Empty,
                    r.PageTitle??string.Empty,
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }


        #region [ UPDATE STATUS ]      
        [HttpGet]
        [CustomAuthorization(AppPermissions.Pages_Administration_StaticPage, AppPermissions.Action_IsCreate)]
        public async Task<IActionResult> UpdateStatus(string id)
        {
            var staticpageObj = _contentManagerService.GetstaticpageById(Convert.ToInt32(id));
            if (staticpageObj != null)
            {
                staticpageObj.IsActive = !staticpageObj.IsActive;
                await _contentManagerService.UpdatestaticPage(staticpageObj);
            }
            else
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "Record not found", IsSuccess = false });
            }
            return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "Status Updated Successfully", IsSuccess = true });
        }
        #endregion [ UPDATE STATUS ]

        #endregion
    }
}
