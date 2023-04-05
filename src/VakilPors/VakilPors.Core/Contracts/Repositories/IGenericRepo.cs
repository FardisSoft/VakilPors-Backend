
using X.PagedList;

namespace VakilPors.Contracts.Repositories;

public interface IGenericRepo<TEntity> where TEntity : class
{
    public IQueryable<TEntity> AsQueryable();
    public IQueryable<TEntity> AsQueryableNoTracking();
    public Task<IPagedList<TEntity>> GetAllPaged(int pageNumber, int pageSize);
    public Task<TEntity> FindAsync(object key);
    public Task<TEntity> FindAsync(object[] keyValues);
    public Task AddAsync(TEntity entity);
    public Task AddRangeAsync(IEnumerable<TEntity> entities);
    public void Update(TEntity entity);
    public void Remove(TEntity entity);
    public void RemoveRange(IEnumerable<TEntity> entities);
}
