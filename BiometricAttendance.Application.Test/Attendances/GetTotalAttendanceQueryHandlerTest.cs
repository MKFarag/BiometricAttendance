using BiometricAttendance.Application.Features.Attendances.GetTotalAttendance;

namespace BiometricAttendance.Application.Test.Attendances;

public class GetTotalAttendanceQueryHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IGenericRepository<Course> _courseRepo = A.Fake<IGenericRepository<Course>>();
    private readonly IGenericRepository<Attendance> _attendanceRepo = A.Fake<IGenericRepository<Attendance>>();
    private readonly IStudentRepository _studentRepo = A.Fake<IStudentRepository>();
    private readonly GetTotalAttendanceQueryHandler _handler;

    public GetTotalAttendanceQueryHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Courses).Returns(_courseRepo);
        A.CallTo(() => _unitOfWork.Attendances).Returns(_attendanceRepo);
        A.CallTo(() => _unitOfWork.Students).Returns(_studentRepo);
        _handler = new GetTotalAttendanceQueryHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenCourseNotFound_ReturnsNotFoundError()
    {
        // Arrange
        A.CallTo(() => _courseRepo.FindAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns((Course?)null);

        var query = new GetTotalAttendanceQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(CourseErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenNoAttendanceRecords_ReturnsEmptyList()
    {
        // Arrange
        var course = Course.Create("Mathematics", "MATH101", 1);

        A.CallTo(() => _courseRepo.FindAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(course);

        A.CallTo(() => _attendanceRepo.FindAllAsync(
                A<Expression<Func<Attendance, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([]);

        var query = new GetTotalAttendanceQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_WhenStudentAttendedAllWeeks_Returns100PercentAttendance()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 2, 1);
        student.Id = 10;
        student.SetName("Ahmed Ali");
        var course = Course.Create("Mathematics", "MATH101", 1);

        var attendances = new List<Attendance>
        {
            Attendance.Create(student.Id, course.Id, 1),
            Attendance.Create(student.Id, course.Id, 2),
            Attendance.Create(student.Id, course.Id, 3),
        };

        A.CallTo(() => _courseRepo.FindAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(course);

        A.CallTo(() => _attendanceRepo.FindAllAsync(
                A<Expression<Func<Attendance, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(attendances);

        A.CallTo(() => _studentRepo.FindAllWithNameAsync(A<Expression<Func<Student, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns([student]);

        var query = new GetTotalAttendanceQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        var summary = result.Value.Single();
        Assert.Equal("100.00%", summary.TotalAttendancePercentage);
        Assert.Equal(student.Name, summary.StudentName);
    }

    [Fact]
    public async Task Handle_WhenMultipleStudents_ReturnsOneEntryPerStudent()
    {
        // Arrange
        var student1 = Student.Create(Guid.CreateVersion7().ToString(), 2, 1);
        student1.Id = 1;
        student1.SetName("Ahmed Ali");
        var student2 = Student.Create(Guid.CreateVersion7().ToString(), 2, 1);
        student2.Id = 2;
        student2.SetName("Sara Khaled");
        var course = Course.Create("Physics", "PHY101", 1);

        var attendances = new List<Attendance>
        {
            Attendance.Create(student1.Id, course.Id, 1),
            Attendance.Create(student1.Id, course.Id, 2),
            Attendance.Create(student2.Id, course.Id, 3),
        };

        A.CallTo(() => _courseRepo.FindAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(course);

        A.CallTo(() => _attendanceRepo.FindAllAsync(
                A<Expression<Func<Attendance, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(attendances);

        A.CallTo(() => _studentRepo.FindAllWithNameAsync(A<Expression<Func<Student, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns([student1, student2]);

        var query = new GetTotalAttendanceQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
    }
}
