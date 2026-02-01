using TasteBox.Abstractions;
using TasteBox.Contracts.Unit;

namespace TasteBox.Interfaces;

public interface IUnitService
{
    Task<Result<List<UnitResponse>>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result<List<UnitResponse>>> GetByTypeAsync(UnitType type, CancellationToken cancellationToken = default);

    Task<Result<UnitResponse>> GetAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<UnitResponse>> AddAsync(CreateUnitRequest request, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(int id, UpdateUnitRequest request, CancellationToken cancellationToken = default);

    Task<Result> ToggleStatusAsync(int id, CancellationToken cancellationToken = default);
}