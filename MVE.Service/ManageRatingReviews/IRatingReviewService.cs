using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.DataTable.Search;

namespace MVE.Service
{
    public interface IRatingReviewService
    {
        List<Rating> GetRatingList();
        PagedListResult<Rating> GetRatingList(SearchQuery<Rating> query, out int totalItems);
        Rating GetRatingById(long Id);
        Rating GetRatingByUserandPackageId(long packageId, long userId);
        Task<Rating> UpdateRatingMaster(Rating entity);
        Task<Rating> SaveRatingMaster(Rating entity);
        Review GetReviewById(long packageId, long userId);
        Task<Review> UpdateReview(Review entity);
        Task<Review> SaveReview(Review entity);
    }
}
