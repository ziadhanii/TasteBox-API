using TasteBox.Contracts.Notification;

namespace TasteBox.Services;

using FirebaseAdmin.Messaging;

public class NotificationService : INotificationService
{
    public async Task SendAsync(SendNotificationRequest request)
    {
        var message = new Message()
        {
            Notification = new Notification
            {
                Title = request.Title,
                Body = request.Body
            },
            Token = request.DeviceToken
        };

        await FirebaseMessaging.DefaultInstance.SendAsync(message);
    }
}