namespace BiometricAttendance.Infrastructure.Authentication.Filters;

public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
{
}
