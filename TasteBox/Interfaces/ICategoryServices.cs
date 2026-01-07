namespace TasteBox.Interfaces;

public interface ICategoryServices
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}