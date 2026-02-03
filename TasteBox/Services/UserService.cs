using TasteBox.Abstractions;
using TasteBox.Abstractions.Consts;
using TasteBox.Contracts.Users;

namespace TasteBox.Services;

public class UserService(
    UserManager<ApplicationUser> userManager,
    IRoleService roleService,
    ApplicationDbContext context) : IUserService
{
    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await (from u in context.Users
               join ur in context.UserRoles
                   on u.Id equals ur.UserId
               join r in context.Roles
                   on ur.RoleId equals r.Id into roles
               where !roles.Any(x => x.Name == DefaultRoles.Customer)
               select new
               {
                   u.Id,
                   u.FirstName,
                   u.LastName,
                   u.Email,
                   u.IsDisabled,
                   Roles = roles.Select(x => x.Name!).ToList()
               }
            )
            .GroupBy(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.IsDisabled })
            .Select(u => new UserResponse
            (
                u.Key.Id,
                u.Key.FirstName,
                u.Key.LastName,
                u.Key.Email,
                u.Key.IsDisabled,
                u.SelectMany(x => x.Roles)
            ))
            .ToListAsync(cancellationToken);

    public async Task<Result<UserResponse>> GetAsync(string id)
    {
        if (await userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure<UserResponse>(UserErrors.UserNotFound);

        var userRoles = await userManager.GetRolesAsync(user);

        var response = (user, userRoles).Adapt<UserResponse>();

        return Result.Success(response);
    }

    public async Task<Result<UserResponse>> AddAsync(CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var emailIsExists = await userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (emailIsExists)
            return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

        var allowedRoles = await roleService.GetAllAsync(cancellationToken: cancellationToken);

        if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

        var user = request.Adapt<ApplicationUser>();

        var result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await userManager.AddToRolesAsync(user, request.Roles);

            var response = (user, request.Roles).Adapt<UserResponse>();

            return Result.Success(response);
        }

        var error = result.Errors.First();

        return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> UpdateAsync(string id, UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var emailIsExists =
            await userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id, cancellationToken);

        if (emailIsExists)
            return Result.Failure(UserErrors.DuplicatedEmail);

        var allowedRoles = await roleService.GetAllAsync(cancellationToken: cancellationToken);

        if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
            return Result.Failure(UserErrors.InvalidRoles);

        if (await userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        user = request.Adapt(user);

        var result = await userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            await context.UserRoles
                .Where(x => x.UserId == id)
                .ExecuteDeleteAsync(cancellationToken);

            await userManager.AddToRolesAsync(user, request.Roles);

            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ToggleStatusAsync(string id)
    {
        if (await userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        user.IsDisabled = !user.IsDisabled;

        var result = await userManager.UpdateAsync(user);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> UnlockAsync(string id)
    {
        if (await userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        var result = await userManager.SetLockoutEndDateAsync(user, null);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }


    public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId)
    {
        var user = await userManager.Users
            .Where(x => x.Id == userId)
            .ProjectToType<UserProfileResponse>()
            .SingleAsync();

        return Result.Success(user);
    }

    public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure(UserErrors.UserNotFound);

        if (!string.IsNullOrWhiteSpace(request.UserName))
        {
            var userNameExists = await userManager.Users
                .AnyAsync(u => u.UserName == request.UserName && u.Id != userId);

            if (userNameExists)
                return Result.Failure(UserErrors.DuplicatedUserName);
        }

        request.Adapt(user);

        var result = await userManager.UpdateAsync(user);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await userManager.FindByIdAsync(userId);

        var result = await userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result<IEnumerable<AddressResponse>>> GetAddressesAsync(string userId)
    {
        var user = await context.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return Result.Failure<IEnumerable<AddressResponse>>(UserErrors.UserNotFound);

        var addresses = user.Addresses.Adapt<IEnumerable<AddressResponse>>();
        return Result.Success(addresses);
    }

    public async Task<Result<AddressResponse>> AddAddressAsync(
        string userId,
        AddressRequest request)
    {
        var user = await context.Users
            .Where(u => u.Id == userId)
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync();

        if (user is null)
            return Result.Failure<AddressResponse>(UserErrors.UserNotFound);

        var address = request.Adapt<Address>();

        if (request.IsDefault && user.Addresses.Count > 0)
        {
            foreach (var addr in user.Addresses)
                addr.IsDefault = false;
        }

        user.Addresses.Add(address);

        await context.SaveChangesAsync();

        return Result.Success(address.Adapt<AddressResponse>());
    }


    public async Task<Result> UpdateAddressAsync(string userId, int addressId, AddressRequest request)
    {
        var user = await context.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return Result.Failure(UserErrors.UserNotFound);

        var address = user.Addresses.FirstOrDefault(a => a.Id == addressId);

        if (address is null)
            return Result.Failure(new Error("Address.NotFound", "Address not found", StatusCodes.Status404NotFound));

        request.Adapt(address);

        if (request.IsDefault)
        {
            foreach (var addr in user.Addresses.Where(a => a.Id != addressId))
                addr.IsDefault = false;
        }

        await context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> DeleteAddressAsync(string userId, int addressId)
    {
        var user = await context.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return Result.Failure(UserErrors.UserNotFound);

        var address = user.Addresses.FirstOrDefault(a => a.Id == addressId);

        if (address is null)
            return Result.Failure(new Error("Address.NotFound", "Address not found", StatusCodes.Status404NotFound));

        user.Addresses.Remove(address);
        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> SetDefaultAddressAsync(string userId, int addressId)
    {
        var user = await context.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return Result.Failure(UserErrors.UserNotFound);

        var address = user.Addresses.FirstOrDefault(a => a.Id == addressId);

        if (address is null)
            return Result.Failure(new Error("Address.NotFound", "Address not found", StatusCodes.Status404NotFound));

        foreach (var addr in user.Addresses)
            addr.IsDefault = false;

        address.IsDefault = true;
        await context.SaveChangesAsync();

        return Result.Success();
    }
}