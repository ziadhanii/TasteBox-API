using System;
using System.Threading.Tasks;
using TasteBox.Contracts.Notification;
using TasteBox.Interfaces;
using Microsoft.Extensions.Logging;

namespace TasteBox.Services;

public class NotificationSender(
    IUserDeviceRepository deviceRepository,
    INotificationService notificationService,
    ILogger<NotificationSender> logger) : INotificationSender
{
    public async Task SendToUserAsync(string userId, SendNotificationRequest request)
    {
        var devices = await deviceRepository.GetByUserIdAsync(userId);
        foreach (var device in devices)
        {
            if (string.IsNullOrEmpty(device.FcmToken))
            {
                logger.LogWarning($"Device with ID {device.Id} has empty FCM token.");
                continue;
            }
            try
            {
                request.DeviceToken = device.FcmToken;
                await notificationService.SendAsync(request);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to send notification to device {device.Id}.");
                // Optionally remove invalid/expired tokens
                await deviceRepository.RemoveAsync(device);
            }
        }
    }
}
