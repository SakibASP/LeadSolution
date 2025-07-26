namespace Infrastructure.Interfaces.Lead;

public interface IGenericRepo<T>
{
    Task<IList<T>> GetAllAsync(dynamic parameter);
    Task<IList<T>> GetAllWithTracingAsync(dynamic parameter);
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T data);
    Task AddRangeAsync(IList<T> dataList);
    Task UpdateAsync(T data);
    Task RemoveAsync(int id);
    Task RemoveRangeAsync(IList<T> entities);
}
