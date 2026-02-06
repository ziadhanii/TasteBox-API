namespace TasteBox.Helpers;

public class OrderCalculator(decimal taxRate, decimal shippingCost)
{
    public OrderCalculationResult Calculate(IEnumerable<CartItem> cartItems)
    {
        var subTotal = cartItems.Sum(ci => ci.Quantity * ci.Price);
        var tax = subTotal * taxRate;
        var discount = 0m; // Can be enhanced with promo codes
        var totalAmount = subTotal + shippingCost + tax - discount;

        return new OrderCalculationResult
        {
            SubTotal = subTotal,
            ShippingCost = shippingCost,
            Tax = tax,
            Discount = discount,
            TotalAmount = totalAmount
        };
    }
}