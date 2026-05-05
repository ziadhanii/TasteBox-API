using TasteBox.Contracts.Notification;

namespace TasteBox.Controllers;

[ApiController]
[Route("api/users")]
public class NotificationsController(IUserDeviceRepository deviceRepository, INotificationSender notificationSender)
    : ControllerBase
{
    [Authorize]
    [HttpPost("save-token")]
    public async Task<IActionResult> SaveToken([FromBody] SaveTokenDto dto)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(dto.FcmToken))
            return BadRequest("FCM token is required.");

        // Prevent duplicate tokens
        var existing = await deviceRepository.GetByTokenAsync(dto.FcmToken);
        if (existing != null)
        {
            if (existing.UserId == userId)
                return Ok(); // Already saved for this user
            // Optionally: update ownership if needed
            return BadRequest("Token already registered for another user.");
        }

        var device = new UserDevice
        {
            UserId = userId,
            FcmToken = dto.FcmToken,
            CreatedAt = DateTime.UtcNow
        };
        await deviceRepository.AddAsync(device);
        return Ok();
    }

    [Authorize]
    [HttpPost("test-notification")]
    public async Task<IActionResult> TestNotification([FromBody] SendNotificationRequest dto)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        await notificationSender.SendToUserAsync(userId, dto);
        return Ok(new { message = "Notification sent to all user devices (if any)." });
    }
}