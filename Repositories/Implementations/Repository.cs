using IotControlService.Models;
using IotControlService.Repositories.Interfaces;

namespace IotControlService.Repositories.Implementations;

public class Repository<T>: IRepository<T> where T : BaseEntity
{  
    protected DataContext DbContext { get; }

    public Repository(DataContext context)
    {
        DbContext = context;
    }
    
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await DbContext.Set<T>().FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await DbContext.Set<T>().AddAsync(entity);
    }

    public void Update(T entity)
    {
        DbContext.Set<T>().Update(entity);;
    }

    public void Remove(T entity)
    {
       DbContext.Set<T>().Remove(entity);
    }
}