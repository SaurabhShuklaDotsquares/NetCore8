using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.DataTable.Search;
using MVE.Repo;

namespace MVE.Service
{
    public class AccomodationTypeService : IAccomodationTypeService
    {
        IRepository<AccommodationType> _repoAccommodationType;
        public AccomodationTypeService(IRepository<AccommodationType> repoAccommodationType)
        {
            _repoAccommodationType = repoAccommodationType;
        }

        public PagedListResult<AccommodationType> Get(SearchQuery<AccommodationType> query, out int totalItems)
        {
            return _repoAccommodationType.Search(query, out totalItems);
        }
        public AccommodationType GetAccommodationTypeById(int Id)
        {            
            return _repoAccommodationType.Query().Filter(x => x.Id.Equals(Id)).Get().FirstOrDefault();
        }
        public async Task<AccommodationType> SaveAccommodationType(AccommodationType accommodation)
        {
            await _repoAccommodationType.InsertAsync(accommodation);
            return accommodation;
        }
        public async Task<AccommodationType> UpdateAccommodationType(AccommodationType accommodation)
        {
            await _repoAccommodationType.UpdateAsync(accommodation);
            return accommodation;
        }
        public bool IsAccommodationTypeExists(string name)
        {
            bool isExist = _repoAccommodationType.Query().Filter(x => x.Name.Trim().Replace(" ", "").ToLower().Equals(name.Trim().Replace(" ", "").ToLower()) && !x.IsDeleted).Get().FirstOrDefault() != null;
          //  bool isExist = _repoAccommodationType.Query().Filter(x => x.Name.Trim().Replace(" ", "").ToLower().Equals(name.Trim().Replace(" ", "").ToLower())).Get().FirstOrDefault() != null;
            return isExist;
        }
        public List<SelectListItem> GetAccommodationTypesForDropDown(bool isActive = true)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var _data = _repoAccommodationType.Query().AsNoTracking().Get().Where(x => !x.IsDeleted && x.IsActive).ToList()?.Select(y => new SelectListItem
           // var _data = _repoAccommodationType.Query().AsNoTracking().Get().ToList()?.Select(y => new SelectListItem
            {
                Text = y.Name,
                Value = y.Id.ToString()
            });
            list.AddRange(_data);
            return list;
        }
    }
}
