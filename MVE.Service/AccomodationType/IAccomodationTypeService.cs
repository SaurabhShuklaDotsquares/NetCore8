using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.DataTable.Search;

namespace MVE.Service
{
    public interface IAccomodationTypeService
    {
        PagedListResult<AccommodationType> Get(SearchQuery<AccommodationType> query, out int totalItems);
        AccommodationType GetAccommodationTypeById(int Id);
        Task<AccommodationType> SaveAccommodationType(AccommodationType accommodation);
        Task<AccommodationType> UpdateAccommodationType(AccommodationType accommodation);
        bool IsAccommodationTypeExists(string name);
        List<SelectListItem> GetAccommodationTypesForDropDown(bool isActive = true);
    }
}
