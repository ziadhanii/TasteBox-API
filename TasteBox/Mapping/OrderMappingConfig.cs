namespace TasteBox.Mapping;

public class OrderMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Order, OrderResponse>()
            .Map(dest => dest.OrderItems, src => src.OrderItems);

        config.NewConfig<OrderItem, OrderItemResponse>();

        config.NewConfig<Order, OrderSummaryResponse>()
            .Map(dest => dest.ItemsCount, src => src.OrderItems.Count);
    }
}