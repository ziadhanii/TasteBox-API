namespace TasteBox.Contracts.Stock;

public record StockResponse(
    int ProductId,
    string ProductName,
    decimal CurrentQuantity,
    decimal ReorderLevel,
    bool IsLowStock,
    decimal UnitCostPrice,
    decimal UnitSellingPrice,
    decimal TotalCostValue,
    decimal TotalSellingValue
);