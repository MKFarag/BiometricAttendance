using BiometricAttendance.Application.Features.Attendances.GetWeekAttendance;

namespace BiometricAttendance.Application.Test.Attendances;

public class GetWeekAttendanceQueryHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IGenericRepository<Course> _courseRepo = A.Fake<IGenericRepository<Course>>();
    private readonly IGenericRepository<Attendance> _attendanceRepo = A.Fake<IGenericRepository<Attendance>>();
    private readonly IStudentRepository _studentRepo = A.Fake<IStudentRepository>();
    private readonly GetWeekAttendanceQueryHandler _handler;

    public GetWeekAttendanceQueryHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Courses).Returns(_courseRepo);
        A.CallTo(() => _unitOfWork.Attendances).Returns(_attendanceRepo);
        A.CallTo(() => _unitOfWork.Students).Returns(_studentRepo);
        _handler = new GetWeekAttendanceQueryHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenCourseNotFound_ReturnsNotFoundError()
    {
        // Arrange
        A.CallTo(() => _courseRepo.FindAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns((Course?)null);

        var query = new GetWeekAttendanceQuery(1, 1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(CourseErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenCourseExists_ReturnsAttendanceRecords()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 2, 1);
        student.Id = 5;
        student.SetName("Ahmed Ali");
        var course = Course.Create("Mathematics", "MATH101", 1);
        course.Id = 1;
        var attendance = Attendance.Create(student.Id, course.Id, 1);

        A.CallTo(() => _courseRepo.FindAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(course);

        A.CallTo(() => _attendanceRepo.FindAllAsync(
                A<Expression<Func<Attendance, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([attendance]);

        A.CallTo(() => _studentRepo.FindAllWithNameAsync(A<Expression<Func<Student, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns([student]);

        var query = new GetWeekAttendanceQuery(1, 1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        Assert.Equal(student.Name, result.Value.First().StudentName);
        Assert.Equal(course.Name, result.Value.First().CourseName);
    }

    [Fact]
    public async Task Handle_WhenNoAttendanceForWeek_ReturnsEmptyList()
    {
        // Arrange
        var course = Course.Create("Mathematics", "MATH101", 1);

        A.CallTo(() => _courseRepo.FindAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(course);

        A.CallTo(() => _attendanceRepo.FindAllAsync(
                A<Expression<Func<Attendance, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([]);

        var query = new GetWeekAttendanceQuery(1, 5);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }
}
