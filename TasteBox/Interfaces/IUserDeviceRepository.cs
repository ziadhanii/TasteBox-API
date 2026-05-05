namespace TasteBox.Interfaces;

public interface IUserDeviceRepository
{
    Task AddAsync(UserDevice device);
    Task<UserDevice?> GetByTokenAsync(string fcmToken);
    Task<List<UserDevice>> GetByUserIdAsync(string userId);
    Task RemoveAsync(UserDevice device);
}