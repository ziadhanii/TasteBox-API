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

    // 🔥 Tracking timestamps
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? PreparingAt { get; set; }
    public DateTime? OutForDeliveryAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = [];

    public Result ChangeStatus(
        OrderStatus newStatus,
        Dictionary<OrderStatus, OrderStatus[]> allowedTransitions)
    {
        if (!allowedTransitions.TryGetValue(Status, out var allowedStatuses)
            || !allowedStatuses.Contains(newStatus))
        {
            return Result.Failure(OrderErrors.InvalidStatusTransition);
        }

        Status = newStatus;

        var now = DateTime.UtcNow;

        switch (newStatus)
        {
            case OrderStatus.Confirmed:
                ConfirmedAt ??= now;
                break;

            case OrderStatus.Preparing:
                PreparingAt ??= now;
                break;

            case OrderStatus.OutForDelivery:
                OutForDeliveryAt ??= now;
                break;

            case OrderStatus.Delivered:
                DeliveredAt ??= now;
                break;

            case OrderStatus.Cancelled:
                CancelledAt ??= now;
                break;
        }

        return Result.Success();
    }

    public IEnumerable<OrderStatusStep> GetTimeline()
    {
        if (Status == OrderStatus.Cancelled)
        {
            return new List<OrderStatusStep>
            {
                new(OrderStatus.Pending, OrderDate, true),
                new(OrderStatus.Cancelled, CancelledAt, true)
            };
        }

        var steps = new List<OrderStatusStep>
        {
            new(OrderStatus.Pending, OrderDate, true),

            new(OrderStatus.Confirmed, ConfirmedAt, ConfirmedAt.HasValue),

            new(OrderStatus.Preparing, PreparingAt, PreparingAt.HasValue),

            new(OrderStatus.OutForDelivery, OutForDeliveryAt, OutForDeliveryAt.HasValue),

            new(OrderStatus.Delivered, DeliveredAt, DeliveredAt.HasValue)
        };

        return steps;
    }

    public bool CanBeCancelled()
        => Status is OrderStatus.Pending or OrderStatus.Confirmed;
}