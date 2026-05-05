using TasteBox.Contracts.Notification;

namespace TasteBox.Interfaces;

public interface INotificationService
{
    Task SendAsync(SendNotificationRequest request);
}