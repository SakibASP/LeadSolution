namespace Infrastructure.Interfaces.Common;

public interface IGenericRepo<T>
{
    Task<IList<T>> GetAllAsync();
    Task<IList<T>> GetAllWithTracingAsync();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T data);
    Task AddRangeAsync(IList<T> dataList);
    Task UpdateAsync(T data);
    Task RemoveAsync(int id);
    Task RemoveRangeAsync(IList<T> entities);
}
