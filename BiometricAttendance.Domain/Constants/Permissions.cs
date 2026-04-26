namespace BiometricAttendance.Domain.Constants;

public static class Permissions
{
    public static string Type { get; } = nameof(Permissions).ToLower();

    #region Attendance

    public const string ReadAttendance = "attendance:read";
    public const string MarkAttendance = "attendance:mark";

    #endregion

    #region Course

    public const string ReadCourse = "course:read";
    public const string AddCourse = "course:add";
    public const string UpdateCourse = "course:modify";
    public const string RemoveCourse = "course:remove";

    #endregion

    #region Department

    public const string ReadDepartment = "department:read";
    public const string AddDepartment = "department:add";
    public const string UpdateDepartment = "department:modify";
    public const string RemoveDepartment = "department:remove";

    #endregion

    #region Fingerprint

    public const string FingerprintAction = "fingerprint:action";
    public const string FingerprintRegister = "fingerprint:register";

    #endregion

    #region Role

    public const string ReadRole = "role:read";
    public const string AddRole = "role:add";
    public const string UpdateRole = "role:modify";
    public const string ToggleRoleStatus = "role:toggle-status";

    #endregion

    #region Students

    public const string ReadStudent = "student:read";
    public const string ChangeStudentDepartment = "student:change-department";
    public const string PromoteStudent = "student:promote";

    #endregion

    #region User

    public const string ReadUser = "user:read";
    public const string AddUser = "user:add";
    public const string UpdateUser = "user:modify";
    public const string ToggleUserStatus = "user:toggle-status";

    #endregion

    public static IList<string?> GetAll()
        => [.. typeof(Permissions).GetFields().Select(x => x.GetValue(x) as string)];
}
