namespace TasteBox.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/me")]
public class AccountController(IUserService userService) : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await userService.GetProfileAsync(User.GetUserId()!);

        return Ok(result.Value);
    }

    [HttpPut("info")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        await userService.UpdateProfileAsync(User.GetUserId()!, request);

        return NoContent();
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await userService.ChangePasswordAsync(User.GetUserId()!, request);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpGet("addresses")]
    public async Task<IActionResult> GetAddresses()
    {
        var result = await userService.GetAddressesAsync(User.GetUserId()!);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("addresses")]
    public async Task<IActionResult> AddAddress([FromBody] AddressRequest request)
    {
        var result = await userService.AddAddressAsync(User.GetUserId()!, request);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("addresses/{addressId}")]
    public async Task<IActionResult> UpdateAddress([FromRoute] int addressId, [FromBody] AddressRequest request)
    {
        var result = await userService.UpdateAddressAsync(User.GetUserId()!, addressId, request);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("addresses/{addressId}")]
    public async Task<IActionResult> DeleteAddress([FromRoute] int addressId)
    {
        var result = await userService.DeleteAddressAsync(User.GetUserId()!, addressId);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("addresses/{addressId}/set-default")]
    public async Task<IActionResult> SetDefaultAddress([FromRoute] int addressId)
    {
        var result = await userService.SetDefaultAddressAsync(User.GetUserId()!, addressId);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}