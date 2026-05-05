namespace TasteBox.Services;

public class UserDeviceRepository(ApplicationDbContext context) : IUserDeviceRepository
{
    public async Task AddAsync(UserDevice device)
    {
        context.Add(device);
        await context.SaveChangesAsync();
    }

    public async Task<UserDevice?> GetByTokenAsync(string fcmToken)
        => await context.Set<UserDevice>().FirstOrDefaultAsync(x => x.FcmToken == fcmToken);

    public async Task<List<UserDevice>> GetByUserIdAsync(string userId)
        => await context.Set<UserDevice>().Where(x => x.UserId == userId).ToListAsync();

    public async Task RemoveAsync(UserDevice device)
    {
        context.Remove(device);
        await context.SaveChangesAsync();
    }
}