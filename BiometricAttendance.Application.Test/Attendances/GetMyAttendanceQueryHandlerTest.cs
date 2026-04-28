using BiometricAttendance.Application.Features.Attendances.GetMyAttendance;

namespace BiometricAttendance.Application.Test.Attendances;

public class GetMyAttendanceQueryHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IStudentRepository _studentRepo = A.Fake<IStudentRepository>();
    private readonly IGenericRepository<Course> _courseRepo = A.Fake<IGenericRepository<Course>>();
    private readonly IGenericRepository<Attendance> _attendanceRepo = A.Fake<IGenericRepository<Attendance>>();
    private readonly IGenericRepository<StudentCourse> _studentCourseRepo = A.Fake<IGenericRepository<StudentCourse>>();
    private readonly GetMyAttendanceQueryHandler _handler;

    public GetMyAttendanceQueryHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Students).Returns(_studentRepo);
        A.CallTo(() => _unitOfWork.Courses).Returns(_courseRepo);
        A.CallTo(() => _unitOfWork.Attendances).Returns(_attendanceRepo);
        A.CallTo(() => _unitOfWork.StudentCourses).Returns(_studentCourseRepo);
        _handler = new GetMyAttendanceQueryHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenStudentNotFound_ReturnsNotFoundError()
    {
        // Arrange
        A.CallTo(() => _studentRepo.FindAsync(
                A<Expression<Func<Student, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns((Student?)null);

        var query = new GetMyAttendanceQuery("nonexistent-user-id");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(StudentErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenStudentHasNoCourses_ReturnsEmptyWithZeroPercentage()
    {
        // Arrange
        var userId = Guid.CreateVersion7().ToString();
        var student = Student.Create(userId, 2, 1);

        A.CallTo(() => _studentRepo.FindAsync(
                A<Expression<Func<Student, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(student);

        A.CallTo(() => _studentCourseRepo.FindAllProjectionAsync(
                A<Expression<Func<StudentCourse, bool>>>.Ignored,
                A<Expression<Func<StudentCourse, int>>>.Ignored,
                A<bool>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([]);

        var query = new GetMyAttendanceQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Attendances);
        Assert.Equal("0.00%", result.Value.TotalPercentage);
    }

    [Fact]
    public async Task Handle_WhenStudentAttendedAllWeeksInAllCourses_Returns100PercentTotal()
    {
        // Arrange
        var userId = Guid.CreateVersion7().ToString();
        var student = Student.Create(userId, 2, 1);
        student.Id = 1;

        var course1 = Course.Create("Mathematics", "MATH101", 1);
        course1.Id = 10;
        var course2 = Course.Create("Physics", "PHY101", 1);
        course2.Id = 20;

        var courseIds = new List<int> { course1.Id, course2.Id };

        var attendances = new List<Attendance>
        {
            Attendance.Create(student.Id, course1.Id, 1),
            Attendance.Create(student.Id, course1.Id, 2),
            Attendance.Create(student.Id, course2.Id, 1),
            Attendance.Create(student.Id, course2.Id, 2),
        };

        A.CallTo(() => _studentRepo.FindAsync(
                A<Expression<Func<Student, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(student);

        A.CallTo(() => _studentCourseRepo.FindAllProjectionAsync(
                A<Expression<Func<StudentCourse, bool>>>.Ignored,
                A<Expression<Func<StudentCourse, int>>>.Ignored,
                A<bool>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(courseIds);

        A.CallTo(() => _courseRepo.FindAllAsync(
                A<Expression<Func<Course, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([course1, course2]);

        A.CallTo(() => _attendanceRepo.FindAllAsync(
                A<Expression<Func<Attendance, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(attendances);

        var query = new GetMyAttendanceQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Attendances.Count);
        Assert.Equal("100.00%", result.Value.TotalPercentage);
        Assert.All(result.Value.Attendances, a => Assert.Equal("100.00%", a.TotalAttendancePercentage));
    }

    [Fact]
    public async Task Handle_WhenStudentMissedSomeWeeks_ReturnsCorrectPercentages()
    {
        // Arrange
        var userId = Guid.CreateVersion7().ToString();
        var student = Student.Create(userId, 3, 1);
        student.Id = 1;
        var otherStudentId = 2;

        var course = Course.Create("Chemistry", "CHEM101", 1);
        course.Id = 5;

        var courseIds = new List<int> { course.Id };

        var attendances = new List<Attendance>
        {
            Attendance.Create(student.Id, course.Id, 1),
            Attendance.Create(student.Id, course.Id, 2),
            Attendance.Create(otherStudentId, course.Id, 3),
        };

        A.CallTo(() => _studentRepo.FindAsync(
                A<Expression<Func<Student, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(student);

        A.CallTo(() => _studentCourseRepo.FindAllProjectionAsync(
                A<Expression<Func<StudentCourse, bool>>>.Ignored,
                A<Expression<Func<StudentCourse, int>>>.Ignored,
                A<bool>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(courseIds);

        A.CallTo(() => _courseRepo.FindAllAsync(
                A<Expression<Func<Course, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([course]);

        A.CallTo(() => _attendanceRepo.FindAllAsync(
                A<Expression<Func<Attendance, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(attendances);

        var query = new GetMyAttendanceQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Attendances);
        Assert.Equal("66.67%", result.Value.Attendances[0].TotalAttendancePercentage);
        Assert.Equal("66.67%", result.Value.TotalPercentage);
    }

    [Fact]
    public async Task Handle_WhenCourseHasNoAttendanceRecords_ReturnsZeroPercentage()
    {
        // Arrange
        var userId = Guid.CreateVersion7().ToString();
        var student = Student.Create(userId, 2, 1);
        student.Id = 1;

        var course = Course.Create("History", "HIST101", 1);
        course.Id = 7;

        var courseIds = new List<int> { course.Id };

        A.CallTo(() => _studentRepo.FindAsync(
                A<Expression<Func<Student, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(student);

        A.CallTo(() => _studentCourseRepo.FindAllProjectionAsync(
                A<Expression<Func<StudentCourse, bool>>>.Ignored,
                A<Expression<Func<StudentCourse, int>>>.Ignored,
                A<bool>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(courseIds);

        A.CallTo(() => _courseRepo.FindAllAsync(
                A<Expression<Func<Course, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([course]);

        A.CallTo(() => _attendanceRepo.FindAllAsync(
                A<Expression<Func<Attendance, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([]);

        var query = new GetMyAttendanceQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Attendances);
        Assert.Equal("0.00%", result.Value.Attendances[0].TotalAttendancePercentage);
        Assert.Equal("0.00%", result.Value.TotalPercentage);
    }
}