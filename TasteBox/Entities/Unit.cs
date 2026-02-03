namespace TasteBox.Entities;

public class Unit : AuditableEntity, ISoftDelete
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public UnitType Type { get; set; }

    public bool IsBaseUnit { get; set; }
    public decimal ConversionFactorToBaseUnit { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}