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
    public class RatingReviewService : IRatingReviewService
    {
        IRepository<Rating> _repoRating;
        IRepository<Review> _repoReview;
        public RatingReviewService(IRepository<Rating> repoRating, IRepository<Review> repoReview)
        {
            _repoRating = repoRating;
            _repoReview = repoReview;
        }
        #region 
        public List<Rating> GetRatingList()
        {
            return _repoRating.Query().AsTracking()
               .Get().ToList();
        }
        public PagedListResult<Rating> GetRatingList(SearchQuery<Rating> query, out int totalItems)
        {
            return _repoRating.Search(query, out totalItems);
        }
        public Rating GetRatingById(long Id)
        {
            return _repoRating.Query()
                .Include(u=>u.User)
                .Get().Where(x => x.Id == Id).FirstOrDefault();
        }
        public Review GetReviewById(long packageId, long userId)
        {
            return _repoReview.Query().Get().Where(x => x.PackageId == packageId && x.UserId == userId).FirstOrDefault();
        }
        
        public Rating GetRatingByUserandPackageId(long packageId, long userId)
        {
            return _repoRating.Query().Get().Where(x => x.PackageId == packageId && x.UserId == userId).FirstOrDefault();
        }
        public async Task<Rating> UpdateRatingMaster(Rating entity)
        {
            await _repoRating.UpdateAsync(entity);
            return entity;
        }
        public async Task<Rating> SaveRatingMaster(Rating entity)
        {
            await _repoRating.InsertAsync(entity);
            return entity;
        }
        public async Task<Review> SaveReview(Review entity)
        {
            await _repoReview.InsertAsync(entity);
            return entity;
        }
        public async Task<Review> UpdateReview(Review entity)
        {
            await _repoReview.UpdateAsync(entity);
            return entity;
        }
        #endregion
    }
}
