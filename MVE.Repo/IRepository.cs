using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.DataTable.Search;

namespace MVE.Repo
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity FindById(object id);

        void Insert(TEntity entity);

        void InsertList(List<TEntity> entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);
        Task<TEntity> UpdateAsyncNew(TEntity entity);
        Task Delete(object id);

        Task DeleteWithoutAsync(long Id);
        // void Delete(object id);
        //PagedListResult<TEntity> Search(SearchQuery<TEntity> searchQuery, out int totalCount);

        Task InsertAsync(TEntity entity);

        Task<List<TEntity>> GetAllAsync();

        Task<TEntity> GetAsync(int id);

        Task UpdateAsync(TEntity entity);
        Task UpdateCollectionAsync(List<TEntity> entityCollection);
        void UpdateCollection(List<TEntity> entityCollection);
        Task UpdateNew(TEntity entity);

        RepositoryQuery<TEntity> Query();

        void ChangeEntityCollectionState<T>(ICollection<T> entityCollection, ObjectState state) where T : class;

        void ChangeEntityState(TEntity entity, ObjectState state);

        void UpdateWithoutAttach(TEntity entity);

        IEnumerable<TEntity> Get<TResult>(Expression<Func<TEntity, bool>> filter = null,
                                              Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                             Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                              bool trackingEnabled = false) where TResult : class;

        MultiVendorEcomDbContext GetContext();

        void InsertGraph(TEntity entity);

        string GetOpenConnection();

        void SaveChanges();
        Task SaveChangesAsync();
        void Delete(List<TEntity> entity);

        PagedListResult<TEntity> Search(SearchQuery<TEntity> searchQuery, out int totalCount);

        void Dispose();
        TEntity UpdateUnchangedEntity(TEntity entity);
        Task DeleteRange<T>(IEnumerable<T> entities) where T : class;
        Task InsertGraphAsync(TEntity entity);
    }
}
