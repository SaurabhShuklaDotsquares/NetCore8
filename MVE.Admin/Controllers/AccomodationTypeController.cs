using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVE.Admin.ViewModels;
using MVE.Core;
using MVE.Core.Code.LIBS;
using MVE.Core.Models.Others;
using MVE.Data.Models;
using MVE.DataTable.Extension;
using MVE.DataTable.Search;
using MVE.DataTable.Sort;
using MVE.Service;

namespace MVE.Admin.Controllers
{
    [Authorize]
    public class AccomodationTypeController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IAccomodationTypeService _accomodationService;
        public AccomodationTypeController(IConfiguration configuration, IAccomodationTypeService accomodationService)
        {
            _configuration = configuration;
            _accomodationService = accomodationService;
        }


        public IActionResult Index()
        {
            AccomodationViewModel vm = new AccomodationViewModel();

            return View(vm);
        }
        [HttpPost]
        public ActionResult Index(MVE.DataTable.DataTables.DataTable dataTable)
        {

            List<DataTableRow> table = new List<DataTableRow>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<AccommodationType>();
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower();
                query.AddFilter(q => q.Name.Contains(sSearch) || (q.Description ?? string.Empty).Contains(sSearch));
            }

            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<AccommodationType, string>(q => q.Name, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<AccommodationType, DateTime>(q => q.CreatedOn, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<AccommodationType, DateTime>(q => q.CreatedOn, SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;

            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<AccommodationType> accommodationsobj = _accomodationService.Get(query, out total).Entities;

            foreach (AccommodationType r in accommodationsobj)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    r.Id.ToString(),
                    count.ToString(),
                    r.Name,
                    r.Description??string.Empty,
                    r.CreatedOn.ToString(SiteKeys.DateFormatWithoutTime),
                    r.IsActive.ToString(),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }

        #region [ ADD / EDIT AccomodationType]

        [HttpGet]
        public IActionResult AddEdit(int? id)
        {
            AccomodationViewModel model = new AccomodationViewModel();
            model.Id = id ?? 0;
            if (id.HasValue)
            {
                var accObj = _accomodationService.GetAccommodationTypeById(id.Value);
                if (accObj != null)
                {
                    model.Id = accObj.Id;
                    model.Name = accObj.Name;
                    model.Description = accObj.Description;
                    model.IsActive = accObj.IsActive ? accObj.IsActive : false;
                    //model.ModifiedOn = accObj.ModifiedOn;
                    //model.ModifiedBy = CurrentUser.Id;

                }
            }
            else
            {
                model.IsActive = true;
                //model.CreatedOn = DateTime.UtcNow;
                //model.CreatedBy = CurrentUser.Id;
            }
            return PartialView("_AddEdit", model);
        }


        [HttpPost]
        public async Task<IActionResult> AddEdit(AccomodationViewModel model)
        {
            AccommodationType accommodation = null; bool isUpdate = false;
            if (model.Name != null && model.Id == 0)
            {
                var isNameExist = IsNameExist(model);
                if (isNameExist)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Accommodation Type '" + model.Name + "' is already exists." });
                }
            }
            if (model.Id > 0)
            {
                accommodation = _accomodationService.GetAccommodationTypeById(model.Id);
                if (accommodation != null)
                {
                    isUpdate = true;
                    accommodation.ModifiedOn = DateTime.UtcNow;
                    accommodation.ModifiedBy = CurrentUser.Id;
                }
            }
            else
            {
                accommodation = new AccommodationType();
                accommodation.CreatedOn = DateTime.UtcNow;
                accommodation.CreatedBy = CurrentUser.Id;
            }
            accommodation.Name = model.Name;
            accommodation.Description = model.Description;
            accommodation.IsActive = model.IsActive;
            accommodation = isUpdate ? await _accomodationService.UpdateAccommodationType(accommodation) : await _accomodationService.SaveAccommodationType(accommodation);


            string displayMsg = $"Accommodation Type has been  {(isUpdate ? "updated" : "created")} successfully";
            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg });

        }

        #endregion [ ADD / EDIT ]
        #region [ EXISTS ]

        [HttpPost]
        public JsonResult IsAccommodationTypeExists(string Name, string PreInitialName)
        {
            bool isExist = false;
            if (Name != PreInitialName)
            {
                isExist = _accomodationService.IsAccommodationTypeExists(Name);
            }
            return Json(!isExist);
        }
        public bool IsNameExist(AccomodationViewModel model)
        {
            bool isExist = false;

            isExist = _accomodationService.IsAccommodationTypeExists(model.Name);

            return isExist;
        }

        #endregion [ EXISTS ]

        #region [ UPDATE STATUS ]      
        [HttpGet]
        public async Task<IActionResult> UpdateStatus(string id)
        {
            var accommodationObj = _accomodationService.GetAccommodationTypeById(Convert.ToInt32(id));
            if (accommodationObj != null)
            {
                accommodationObj.IsActive = !accommodationObj.IsActive;
                await _accomodationService.UpdateAccommodationType(accommodationObj);
            }
            else
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "Record not found", IsSuccess = false });
            }
            return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "Status Updated Successfully", IsSuccess = true });
        }
        #endregion [ UPDATE STATUS ]

        #region [ DELETE ]

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Accommodation Type " },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }

            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection FC)
        {
            try
            {
                var Details = _accomodationService.GetAccommodationTypeById(id);

                if (Details != null)
                {
                    Details.IsDeleted = false;
                    await _accomodationService.UpdateAccommodationType(Details);
                    ShowSuccessMessage("Success!", "Accommodation Type deleted successfully.", false);
                }
                else
                {
                    ShowErrorMessage("Error!", "Some Error Occurred ", false);
                }

            }
            catch (Exception ex)
            {
                string message = ex.GetBaseException().Message;
                ShowErrorMessage("Error!", message, false);
            }
            return RedirectToAction("Index");

        }
        #endregion [ DELETE ]
    }
}
