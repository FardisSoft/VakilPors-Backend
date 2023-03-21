
namespace VakilPors.Contracts.Repositories;

public interface IGenericRepo<TEntity> where TEntity : class
{
    public IQueryable<TEntity> AsQueryable();
    public Task<TEntity> FindAsync(object key);
    public Task<TEntity> FindAsync(object[] keyValues);
    public Task AddAsync(TEntity entity);
    public Task AddRangeAsync(IEnumerable<TEntity> entities);
    public void Update(TEntity entity);
    public void Remove(TEntity entity);
    public void RemoveRange(IEnumerable<TEntity> entities);
}
