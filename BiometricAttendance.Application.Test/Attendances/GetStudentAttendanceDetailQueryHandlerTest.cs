using BiometricAttendance.Application.Contracts.Attendances;
using BiometricAttendance.Application.Features.Attendances.GetStudentAttendanceDetail;

namespace BiometricAttendance.Application.Test.Attendances;

public class GetStudentAttendanceDetailQueryHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IStudentRepository _studentRepo = A.Fake<IStudentRepository>();
    private readonly IGenericRepository<Course> _courseRepo = A.Fake<IGenericRepository<Course>>();
    private readonly IGenericRepository<Attendance> _attendanceRepo = A.Fake<IGenericRepository<Attendance>>();
    private readonly GetStudentAttendanceDetailQueryHandler _handler;

    public GetStudentAttendanceDetailQueryHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Students).Returns(_studentRepo);
        A.CallTo(() => _unitOfWork.Courses).Returns(_courseRepo);
        A.CallTo(() => _unitOfWork.Attendances).Returns(_attendanceRepo);
        _handler = new GetStudentAttendanceDetailQueryHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenStudentNotFound_ReturnsNotFoundError()
    {
        // Arrange
        A.CallTo(() => _studentRepo.FindAllWithNameAsync(
                A<Expression<Func<Student, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([]);

        var query = new GetStudentAttendanceDetailQuery(1, 1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(StudentErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenCourseNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 2, 1);
        student.SetName("Ahmed Ali");

        A.CallTo(() => _studentRepo.FindAllWithNameAsync(
                A<Expression<Func<Student, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([student]);

        A.CallTo(() => _courseRepo.FindAsync(
                A<Expression<Func<Course, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns((Course?)null);

        var query = new GetStudentAttendanceDetailQuery(1, 1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(CourseErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenStudentAttendedAllWeeks_Returns100PercentAttendance()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 3, 1);
        student.SetName("Ahmed Ali");
        var course = Course.Create("Mathematics", "MATH101", 1);

        var attendances = new List<Attendance>
        {
            Attendance.Create(student.Id, course.Id, 1),
            Attendance.Create(student.Id, course.Id, 2),
            Attendance.Create(student.Id, course.Id, 3),
        };

        A.CallTo(() => _studentRepo.FindAllWithNameAsync(
                A<Expression<Func<Student, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([student]);

        A.CallTo(() => _courseRepo.FindAsync(
                A<Expression<Func<Course, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(course);

        A.CallTo(() => _attendanceRepo.FindAllAsync(
                A<Expression<Func<Attendance, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(attendances);

        var query = new GetStudentAttendanceDetailQuery(student.Id, course.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("100.00%", result.Value.CourseAttendance.TotalAttendancePercentage);
        Assert.Equal(student.Name, result.Value.Student.Name);
        Assert.Equal(course.Name, result.Value.CourseAttendance.Name);
        Assert.Equal(course.Code, result.Value.CourseAttendance.Code);
    }

    [Fact]
    public async Task Handle_WhenStudentMissedSomeWeeks_ReturnsPartialPercentage()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 3, 1);
        student.Id = 1;
        student.SetName("Ahmed Ali");
        var otherStudentId = 2;
        var course = Course.Create("Physics", "PHY101", 1);

        // Student attended 2 weeks, other student attended 1 week → total 3 distinct weeks
        var attendances = new List<Attendance>
        {
            Attendance.Create(student.Id, course.Id, 1),
            Attendance.Create(student.Id, course.Id, 2),
            Attendance.Create(otherStudentId, course.Id, 3),
        };

        A.CallTo(() => _studentRepo.FindAllWithNameAsync(
                A<Expression<Func<Student, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([student]);

        A.CallTo(() => _courseRepo.FindAsync(
                A<Expression<Func<Course, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(course);

        A.CallTo(() => _attendanceRepo.FindAllAsync(
                A<Expression<Func<Attendance, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(attendances);

        var query = new GetStudentAttendanceDetailQuery(student.Id, course.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("66.67%", result.Value.CourseAttendance.TotalAttendancePercentage);
    }

    [Fact]
    public async Task Handle_WhenNoCourseAttendanceExists_ReturnsZeroPercentage()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 2, 1);
        student.SetName("Ahmed Ali");
        var course = Course.Create("Chemistry", "CHEM101", 1);

        A.CallTo(() => _studentRepo.FindAllWithNameAsync(
                A<Expression<Func<Student, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([student]);

        A.CallTo(() => _courseRepo.FindAsync(
                A<Expression<Func<Course, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(course);

        A.CallTo(() => _attendanceRepo.FindAllAsync(
                A<Expression<Func<Attendance, bool>>>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([]);

        var query = new GetStudentAttendanceDetailQuery(student.Id, course.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("0.00%", result.Value.CourseAttendance.TotalAttendancePercentage);
    }
}
