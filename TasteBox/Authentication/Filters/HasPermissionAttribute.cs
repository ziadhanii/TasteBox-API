using Microsoft.AspNetCore.Authorization;

namespace TasteBox.Authentication.Filters;

public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
{
}