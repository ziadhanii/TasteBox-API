using TasteBox.Abstractions;
using TasteBox.Contracts.Users;

namespace TasteBox.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> GetAsync(string id);
    Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(string id);
    Task<Result> UnlockAsync(string id);
    Task<Result<UserProfileResponse>> GetProfileAsync(string userId);
    Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request);
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);

    Task<Result<IEnumerable<AddressResponse>>> GetAddressesAsync(string userId);
    Task<Result<AddressResponse>> AddAddressAsync(string userId, AddressRequest request);
    Task<Result> UpdateAddressAsync(string userId, int addressId, AddressRequest request);
    Task<Result> DeleteAddressAsync(string userId, int addressId);
    Task<Result> SetDefaultAddressAsync(string userId, int addressId);
}