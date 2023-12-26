using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.Repo;

namespace MVE.Service
{
    public class VisaGuideService : IVisaGuideService
    {
        IRepository<VisaGuide> _repoVisaGuide;
        public VisaGuideService(IRepository<VisaGuide> repoDVisaGuide)
        {
            _repoVisaGuide = repoDVisaGuide;
        }
        public void Dispose()
        {
            if (_repoVisaGuide!=null)
            {
                _repoVisaGuide.Dispose();
            }
        }
        public VisaGuide GetVisaGuidyCountry(int cId)
        {
            return _repoVisaGuide.Query().Filter(x => x.CountryId.Equals(cId)).Get().FirstOrDefault();
        }
        public VisaGuide GetVisaGuidePageByUrl(string url)
        {
            return _repoVisaGuide.Query().Filter(x => x.Url == url && x.IsActive==true).Get().FirstOrDefault();
        }
        public async Task<VisaGuide> UpdateVisaGuide(VisaGuide visa)
        {
            await _repoVisaGuide.UpdateAsync(visa);
            return visa;
        }
        public async Task<VisaGuide> SaveVisaGuide(VisaGuide visa)
        {
            await _repoVisaGuide.InsertAsync(visa);
            return visa;
        }
    }
}
