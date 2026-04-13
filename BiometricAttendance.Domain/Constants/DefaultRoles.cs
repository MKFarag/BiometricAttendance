namespace BiometricAttendance.Domain.Constants;

public static class DefaultRoles
{
    public partial class Admin
    {
        public const string Name = nameof(Admin);
        public const string Id = "019d8540-1a1b-7ee7-899d-305ada12c7a6";
        public const string ConcurrencyStamp = "019d8540-1a1b-7ee7-899d-305ba6339a26";
    }

    public partial class Student
    {
        public const string Name = nameof(Student);
        public const string Id = "019d8540-1a1b-7ee7-899d-305c85cbce80";
        public const string ConcurrencyStamp = "019d8540-1a1b-7ee7-899d-305d89670d46";
    }

    public partial class Instructor
    {
        public const string Name = nameof(Instructor);
        public const string Id = "019d8563-a991-7ed3-85ca-3ce1df3896c2";
        public const string ConcurrencyStamp = "019d8563-d6df-7c2e-9b3d-223f0d1d0465";
    }
}
