namespace BiometricAttendance.Domain.Constants;

public static class DefaultRoles
{
    public partial class Admin
    {
        public const string Name = nameof(Admin);
        public const string Id = "019d8540-1a1b-7ee7-899d-305ada12c7a6";
        public const string ConcurrencyStamp = "019d8540-1a1b-7ee7-899d-305ba6339a26";
    }

    public partial class Pending
    {
        public const string Name = nameof(Pending);
        public const string Id = "019d8a96-d2af-7fd4-be73-922f85b81569";
        public const string ConcurrencyStamp = "019d8a97-499e-78da-8340-b2551ae86a19";
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

    public partial class SuperInstructor
    {
        public const string Name = nameof(SuperInstructor);
        public const string Id = "019dd5a8-a53b-7bab-9115-df41914dc7b6";
        public const string ConcurrencyStamp = "019dd5aa-01b4-70a7-b2b8-a6929b0cfec5";
    }
}
