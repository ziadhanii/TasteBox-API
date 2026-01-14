namespace TasteBox.Entities;

public class Unit
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;

    public decimal ConversionFactorToBaseUnit { get; set; }

    public bool IsBaseUnit { get; set; }
}