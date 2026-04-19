using BiometricAttendance.Application.Features.Courses.Remove;

namespace BiometricAttendance.Application.Test.Courses;

public class RemoveCourseCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IGenericRepository<Course> _courseRepo = A.Fake<IGenericRepository<Course>>();
    private readonly IGenericRepository<StudentCourse> _studentCourseRepo = A.Fake<IGenericRepository<StudentCourse>>();
    private readonly ICacheService _cacheService = A.Fake<ICacheService>();
    private readonly RemoveCourseCommandHandler _handler;

    public RemoveCourseCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Courses).Returns(_courseRepo);
        A.CallTo(() => _unitOfWork.StudentCourses).Returns(_studentCourseRepo);
        _handler = new RemoveCourseCommandHandler(_unitOfWork, _cacheService);
    }

    [Fact]
    public async Task Handle_WhenCourseNotFound_ReturnsNotFoundError()
    {
        // Arrange
        A.CallTo(() => _courseRepo.AnyAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        var command = new RemoveCourseCommand(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(CourseErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenCourseInUseInStudentCourses_ReturnsInUseError()
    {
        // Arrange
        A.CallTo(() => _courseRepo.AnyAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _studentCourseRepo.AnyAsync(A<Expression<Func<StudentCourse, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        var command = new RemoveCourseCommand(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(CourseErrors.InUse, result.Error);

        A.CallTo(() => _courseRepo.ExecuteDeleteAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();
    }


    [Fact]
    public async Task Handle_WhenPassValidData_ReturnsSuccess()
    {
        // Arrange
        A.CallTo(() => _courseRepo.AnyAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _studentCourseRepo.AnyAsync(A<Expression<Func<StudentCourse, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _courseRepo.ExecuteDeleteAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.CompletedTask);

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.CompletedTask);

        var command = new RemoveCourseCommand(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        A.CallTo(() => _courseRepo.ExecuteDeleteAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
