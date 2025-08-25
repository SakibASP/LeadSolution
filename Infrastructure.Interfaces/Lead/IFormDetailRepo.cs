namespace Infrastructure.Interfaces.Lead;

public interface IFormDetailRepo
{
    Task RemoveAsync(int id);
}
