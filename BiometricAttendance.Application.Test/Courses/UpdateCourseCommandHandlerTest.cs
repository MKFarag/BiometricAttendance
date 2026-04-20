using BiometricAttendance.Application.Features.Courses.Update;

namespace BiometricAttendance.Application.Test.Courses;

public class UpdateCourseCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IGenericRepository<Course> _courseRepo = A.Fake<IGenericRepository<Course>>();
    private readonly ICacheService _cacheService = A.Fake<ICacheService>();
    private readonly UpdateCourseCommandHandler _handler;

    public UpdateCourseCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Courses).Returns(_courseRepo);
        _handler = new UpdateCourseCommandHandler(_unitOfWork, _cacheService);
    }

    [Fact]
    public async Task Handle_WhenCourseNotFound_ReturnsNotFoundError()
    {
        // Arrange
        A.CallTo(() => _courseRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns((Course?)null);

        var command = new UpdateCourseCommand(1, "Math", "MATH101", 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(CourseErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenAllValuesEqualsCurrentValues_ReturnsSuccessWithoutChanges()
    {
        // Arrange
        var course = Course.Create("Math", "MATH101", 1);

        A.CallTo(() => _courseRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(course);

        var command = new UpdateCourseCommand(1, "math", "math101", 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        A.CallTo(() => _courseRepo.AnyAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenNameAlreadyExists_ReturnsNameAlreadyExistsError()
    {
        // Arrange
        var course = Course.Create("Math", "MATH101", 1);

        A.CallTo(() => _courseRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(course);

        A.CallTo(() => _courseRepo.AnyAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        var command = new UpdateCourseCommand(course.Id, "Physics", "MATH101", 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(CourseErrors.NameAlreadyExists, result.Error);
    }

    [Fact]
    public async Task Handle_WhenCodeAlreadyExists_ReturnsCodeAlreadyExistsError()
    {
        // Arrange
        var course = Course.Create("Math", "MATH101", 1);

        A.CallTo(() => _courseRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(course);

        A.CallTo(() => _courseRepo.AnyAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        var command = new UpdateCourseCommand(course.Id, "Math", "PHY201", 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(CourseErrors.CodeAlreadyExists, result.Error);
    }

    [Fact]
    public async Task Handle_WhenDepartmentNotFound_ReturnsDepartmentNotFoundError()
    {
        // Arrange
        var course = Course.Create("Math", "MATH101", 1);

        A.CallTo(() => _courseRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(course);

        A.CallTo(() => _courseRepo.AnyAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .ReturnsNextFromSequence(false, false);

        A.CallTo(() => _unitOfWork.Departments.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        var command = new UpdateCourseCommand(course.Id, "Physics", "PHY201", 22);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DepartmentErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenPassValidData_ReturnsSuccess()
    {
        // Arrange
        var course = Course.Create("Math", "MATH101", 1);

        A.CallTo(() => _courseRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(course);

        A.CallTo(() => _courseRepo.AnyAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .ReturnsNextFromSequence(false, false);

        A.CallTo(() => _unitOfWork.Departments.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .Returns(1);

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.CompletedTask);

        var command = new UpdateCourseCommand(course.Id, "Physics", "PHY201", 2);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
