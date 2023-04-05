using Microsoft.EntityFrameworkCore;
using VakilPors.Contracts.Repositories;
using VakilPors.Data.Context;
using X.PagedList;
namespace VakilPors.Data.Repositories;

public class GenericRepo<TEntity> : IGenericRepo<TEntity> where TEntity : class
{
    protected int CurrentUserId; 

    protected DbSet<TEntity> Entities;
    public GenericRepo(AppDbContext appDbContext) 
        => Entities = appDbContext.Set<TEntity>();
    
    public IQueryable<TEntity> AsQueryable()
        => Entities;
    
    public IQueryable<TEntity> AsQueryableNoTracking()
        => Entities.AsNoTracking();
    
    public async Task<IPagedList<TEntity>> GetAllPaged(int pageNumber, int pageSize)
        => await Entities.AsNoTracking().ToPagedListAsync(pageNumber, pageSize);
    public async Task<TEntity> FindAsync(object[] keyValues)
        => await Entities.FindAsync(keyValues);

    public async Task<TEntity> FindAsync(object key)
        => await Entities.FindAsync(key);

    public async Task AddAsync(TEntity entity)
         => await Entities.AddAsync(entity);
    
    public async Task AddRangeAsync(IEnumerable<TEntity> entities) 
        => await Entities.AddRangeAsync(entities);
    
    public void Update(TEntity entity)
        => Entities.Update(entity);
    
    public void Remove(TEntity entity)
        => Entities.Remove(entity);

    public void RemoveRange(IEnumerable<TEntity> entities)
        => Entities.RemoveRange(entities);

    
}

