namespace BiometricAttendance.Domain.Constants;

public static class Cache
{
    public partial class Tags
    {
        public const string Roles = "roles";
        public const string Users = "users";
    }

    public partial class Keys
    {
        public const string RolesWithoutDefault = "roles:without-default";
        public const string UsersWithoutDefaultRole = "users:without-default-role";
    }

    public static class Expirations
    {
        public static readonly TimeSpan Short = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan Medium = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan Long = TimeSpan.FromHours(1);
        public static readonly TimeSpan VeryLong = TimeSpan.FromHours(24);
    }
}
