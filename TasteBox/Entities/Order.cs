using TasteBox.Abstractions;

namespace TasteBox.Entities;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentMethod PaymentMethod { get; set; }

    public decimal SubTotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }

    public string ShippingAddress { get; set; } = string.Empty;
    public string ShippingCity { get; set; } = string.Empty;
    public string ShippingState { get; set; } = string.Empty;
    public string ShippingZipCode { get; set; } = string.Empty;
    public string? ShippingPhone { get; set; }

    public string? Notes { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = [];

    // Domain Methods for Business Logic


    public Result ChangeStatus(OrderStatus newStatus, Dictionary<OrderStatus, OrderStatus[]> allowedTransitions)
    {
        if (!allowedTransitions.TryGetValue(Status, out var allowedStatuses))
            return Result.Failure(OrderErrors.InvalidStatusTransition);

        if (!allowedStatuses.Contains(newStatus))
            return Result.Failure(OrderErrors.InvalidStatusTransition);

        Status = newStatus;

        switch (newStatus)
        {
            case OrderStatus.Confirmed:
                ConfirmedAt = DateTime.UtcNow;
                break;
            case OrderStatus.Shipped:
                ShippedAt = DateTime.UtcNow;
                break;
            case OrderStatus.Delivered:
                DeliveredAt = DateTime.UtcNow;
                break;
            case OrderStatus.Cancelled:
                CancelledAt = DateTime.UtcNow;
                break;
        }

        return Result.Success();
    }

    public bool CanBeCancelled()
    {
        return Status != OrderStatus.Cancelled && Status != OrderStatus.Delivered;
    }
}