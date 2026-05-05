namespace TasteBox.Contracts.Notification;

public class SendNotificationRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string DeviceToken { get; set; } = string.Empty;
}