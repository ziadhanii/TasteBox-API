using Microsoft.AspNetCore.Authorization;

namespace TasteBox.Authentication.Filters;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}