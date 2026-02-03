namespace TasteBox.Mapping;

public class CartMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CartItem, CartItemResponse>()
            .Map(dest => dest.ProductName, src => src.Product.Name)
            .Map(dest => dest.ProductImage, src => src.Product.ImageUrl)
            .Map(dest => dest.UnitPrice, src => src.Product.UnitPrice)
            .Map(dest => dest.DiscountedPrice, src => src.Product.DiscountedPrice)
            .Map(dest => dest.Subtotal, src => src.Quantity * src.Price)
            .Map(dest => dest.UnitName, src => src.Product.Unit.Name)
            .Map(dest => dest.UnitSymbol, src => src.Product.Unit.Symbol)
            .Map(dest => dest.IsWeighedProduct, src => src.Product.IsWeighedProduct)
            .Map(dest => dest.AvailableStock, src => src.Product.Stock.Quantity)
            .Map(dest => dest.MinOrderQty, src => src.Product.MinOrderQty)
            .Map(dest => dest.MaxOrderQty, src => src.Product.MaxOrderQty);

        config.NewConfig<Cart, CartResponse>()
            .Map(dest => dest.Items, src => src.CartItems.Where(ci => !ci.Product.IsDeleted))
            .Map(dest => dest.ItemsCount, src => src.CartItems.Count(ci => !ci.Product.IsDeleted))
            .Map(dest => dest.TotalAmount, src => src.CartItems
                .Where(ci => !ci.Product.IsDeleted)
                .Sum(ci => ci.Quantity * ci.Price));
    }
}