using System.Threading.Tasks;
using TasteBox.Contracts.Notification;

namespace TasteBox.Interfaces;

public interface INotificationSender
{
    Task SendToUserAsync(string userId, SendNotificationRequest request);
}
