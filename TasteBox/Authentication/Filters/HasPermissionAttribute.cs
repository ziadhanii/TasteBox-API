namespace TasteBox.Authentication.Filters;

public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
{
}