using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;

namespace MVE.Service
{
    public interface IVisaGuideService
    {
        VisaGuide GetVisaGuidyCountry(int cId);
        Task<VisaGuide> UpdateVisaGuide(VisaGuide visaGuid);
        Task<VisaGuide> SaveVisaGuide(VisaGuide visaGuid);
        VisaGuide GetVisaGuidePageByUrl(string url);
    }
}
