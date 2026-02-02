namespace TasteBox.Mapping;

public class StockMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Stock, StockResponse>()
            .Map(dest => dest.ProductId, src => src.Product.Id)
            .Map(dest => dest.CurrentQuantity, src => src.Quantity)
            .Map(dest => dest.ReorderLevel, src => src.MinQuantity)
            .Map(dest => dest.UnitCostPrice, src => src.Product.UnitPrice)
            .Map(dest => dest.UnitSellingPrice, src => src.Product.UnitPrice)
            .Map(dest => dest.TotalCostValue, src => src.Quantity * src.Product.CostPrice)
            .Map(dest => dest.TotalSellingValue, src => src.Quantity * src.Product.UnitPrice)
            .Map(dest => dest.ProductName, src => src.Product.Name)
            .Map(dest => dest.IsLowStock, src => src.Quantity <= src.MinQuantity);
    }
}