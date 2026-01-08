using TasteBox.Contracts.Product;

namespace TasteBox.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductResponse>()
            .Map(dest => dest.Unit, src => src.Unit.Name)
            .Map(dest => dest.UnitSymbol, src => src.Unit.Symbol);

    }
}