namespace TasteBox.Settings;

public class OrderSettings
{
    public decimal TaxRate { get; set; } = 0.1m; // 10% tax
    public decimal DefaultShippingCost { get; set; } = 0m;
}