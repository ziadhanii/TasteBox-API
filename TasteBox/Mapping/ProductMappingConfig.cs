namespace TasteBox.Mapping;

public class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductResponse>()
            .Map(dest => dest.UnitName, src => src.Unit.Name)
            .Map(dest => dest.UnitSymbol, src => src.Unit.Symbol)
            .Map(dest => dest.QuantityInStock, src => src.Stock.Quantity)
            .Map(dest => dest.HasDiscount,
                src => src.DiscountedPrice.HasValue && src.DiscountedPrice.Value < src.UnitPrice)
            .Map(dest => dest.IsAvailable, src => src.Stock.Quantity > 0);
    }
}