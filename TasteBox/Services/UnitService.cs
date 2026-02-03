using TasteBox.Abstractions;
using TasteBox.Contracts.Unit;

namespace TasteBox.Services;

public class UnitService(ApplicationDbContext context) : IUnitService
{
    public async Task<Result<List<UnitResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var units = await context.Units
            .AsNoTracking()
            .Where(u => !u.IsDeleted)
            .ProjectToType<UnitResponse>()
            .ToListAsync(cancellationToken);

        return Result.Success(units);
    }

    public async Task<Result<List<UnitResponse>>> GetByTypeAsync(UnitType type,
        CancellationToken cancellationToken = default)
    {
        var units = await context.Units
            .AsNoTracking()
            .Where(u => u.Type == type && !u.IsDeleted)
            .ProjectToType<UnitResponse>()
            .ToListAsync(cancellationToken);

        return Result.Success(units);
    }

    public async Task<Result<UnitResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var unit = await context.Units
            .AsNoTracking()
            .Where(u => u.Id == id && !u.IsDeleted)
            .ProjectToType<UnitResponse>()
            .FirstOrDefaultAsync(cancellationToken);

        return unit is null
            ? Result.Failure<UnitResponse>(UnitErrors.UnitNotFound)
            : Result.Success(unit);
    }

    public async Task<Result<UnitResponse>> AddAsync(CreateUnitRequest request,
        CancellationToken cancellationToken = default)
    {
        var nameExists = await context.Units
            .AnyAsync(u => u.Name == request.Name && !u.IsDeleted, cancellationToken);

        if (nameExists)
            return Result.Failure<UnitResponse>(UnitErrors.UnitNameAlreadyExists);

        var symbolExists = await context.Units
            .AnyAsync(u => u.Symbol == request.Symbol && !u.IsDeleted, cancellationToken);

        if (symbolExists)
            return Result.Failure<UnitResponse>(UnitErrors.UnitSymbolAlreadyExists);

        if (request.IsBaseUnit && request.ConversionFactorToBaseUnit != 1)
            return Result.Failure<UnitResponse>(UnitErrors.BaseUnitConversionMustBeOne);

        if (request.ConversionFactorToBaseUnit <= 0)
            return Result.Failure<UnitResponse>(UnitErrors.InvalidConversionFactor);

        var unit = request.Adapt<Unit>();

        await context.Units.AddAsync(unit, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var response = unit.Adapt<UnitResponse>();
        return Result.Success(response);
    }

    public async Task<Result> UpdateAsync(int id, UpdateUnitRequest request,
        CancellationToken cancellationToken = default)
    {
        var unit = await context.Units
            .FindAsync([id], cancellationToken);

        if (unit is null)
            return Result.Failure(UnitErrors.UnitNotFound);

        var nameExists = await context.Units
            .AnyAsync(u => u.Name == request.Name && u.Id != id && !u.IsDeleted, cancellationToken);

        if (nameExists)
            return Result.Failure(UnitErrors.UnitNameAlreadyExists);

        var symbolExists = await context.Units
            .AnyAsync(u => u.Symbol == request.Symbol && u.Id != id && !u.IsDeleted, cancellationToken);

        if (symbolExists)
            return Result.Failure(UnitErrors.UnitSymbolAlreadyExists);

        if (request.IsBaseUnit && request.ConversionFactorToBaseUnit != 1)
            return Result.Failure(UnitErrors.BaseUnitConversionMustBeOne);

        if (request.ConversionFactorToBaseUnit <= 0)
            return Result.Failure(UnitErrors.InvalidConversionFactor);

        unit.Name = request.Name;
        unit.Symbol = request.Symbol;
        // Preserve existing base-unit designation here to avoid
        // changing conversion semantics without proper validation.
        unit.ConversionFactorToBaseUnit = request.ConversionFactorToBaseUnit;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ToggleStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var unit = await context.Units
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (unit is null)
            return Result.Failure(UnitErrors.UnitNotFound);

        var isInUse = await context.Products
            .AnyAsync(p => p.UnitId == id && !p.IsDeleted, cancellationToken);

        if (isInUse)
            return Result.Failure(UnitErrors.CannotDeactivateUnitInUse);

        unit.IsDeleted = !unit.IsDeleted;
        unit.DeletedAt = unit.IsDeleted ? DateTime.UtcNow : null;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}