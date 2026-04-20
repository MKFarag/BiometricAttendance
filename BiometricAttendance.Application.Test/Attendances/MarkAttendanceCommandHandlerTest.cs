using BiometricAttendance.Application.Features.Attendances.MarkAttendance;

namespace BiometricAttendance.Application.Test.Attendances;

public class MarkAttendanceCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IStudentRepository _studentRepo = A.Fake<IStudentRepository>();
    private readonly IGenericRepository<Course> _courseRepo = A.Fake<IGenericRepository<Course>>();
    private readonly IGenericRepository<StudentCourse> _studentCourseRepo = A.Fake<IGenericRepository<StudentCourse>>();
    private readonly IGenericRepository<Attendance> _attendanceRepo = A.Fake<IGenericRepository<Attendance>>();
    private readonly MarkAttendanceCommandHandler _handler;

    public MarkAttendanceCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Students).Returns(_studentRepo);
        A.CallTo(() => _unitOfWork.Courses).Returns(_courseRepo);
        A.CallTo(() => _unitOfWork.StudentCourses).Returns(_studentCourseRepo);
        A.CallTo(() => _unitOfWork.Attendances).Returns(_attendanceRepo);
        _handler = new MarkAttendanceCommandHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenStudentNotFound_ReturnsNotFoundError()
    {
        // Arrange
        A.CallTo(() => _studentRepo.AnyAsync(A<Expression<Func<Student, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        var command = new MarkAttendanceCommand(1, 1, 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(StudentErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenCourseNotFound_ReturnsNotFoundError()
    {
        // Arrange
        A.CallTo(() => _studentRepo.AnyAsync(A<Expression<Func<Student, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _courseRepo.AnyAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        var command = new MarkAttendanceCommand(1, 1, 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(CourseErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenStudentNotEnrolledInCourse_ReturnsStudentNotEnrolledError()
    {
        // Arrange
        A.CallTo(() => _studentRepo.AnyAsync(A<Expression<Func<Student, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _courseRepo.AnyAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _studentCourseRepo.AnyAsync(A<Expression<Func<StudentCourse, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        var command = new MarkAttendanceCommand(1, 1, 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(AttendanceErrors.StudentNotEnrolled, result.Error);
    }

    [Fact]
    public async Task Handle_WhenAttendanceAlreadyMarked_ReturnsAlreadyMarkedError()
    {
        // Arrange
        A.CallTo(() => _studentRepo.AnyAsync(A<Expression<Func<Student, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _courseRepo.AnyAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _studentCourseRepo.AnyAsync(A<Expression<Func<StudentCourse, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _attendanceRepo.AnyAsync(A<Expression<Func<Attendance, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        var command = new MarkAttendanceCommand(1, 1, 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(AttendanceErrors.AlreadyMarked, result.Error);

        A.CallTo(() => _attendanceRepo.AddAsync(A<Attendance>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_MarksAttendanceAndReturnsSuccess()
    {
        // Arrange
        A.CallTo(() => _studentRepo.AnyAsync(A<Expression<Func<Student, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _courseRepo.AnyAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _studentCourseRepo.AnyAsync(A<Expression<Func<StudentCourse, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _attendanceRepo.AnyAsync(A<Expression<Func<Attendance, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _attendanceRepo.AddAsync(A<Attendance>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Attendance.Create(1, 1, 1));

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .Returns(1);

        var command = new MarkAttendanceCommand(1, 1, 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        A.CallTo(() => _attendanceRepo.AddAsync(A<Attendance>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
