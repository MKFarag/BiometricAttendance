using BiometricAttendance.Application.Contracts.Courses;
using BiometricAttendance.Application.Features.Courses.Add;

namespace BiometricAttendance.Application.Test.Courses;

public class AddCourseCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IGenericRepository<Course> _courseRepo = A.Fake<IGenericRepository<Course>>();
    private readonly ICacheService _cacheService = A.Fake<ICacheService>();
    private readonly AddCourseCommandHandler _handler;

    public AddCourseCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Courses).Returns(_courseRepo);
        _handler = new AddCourseCommandHandler(_unitOfWork, _cacheService);
    }

    [Fact]
    public async Task Handle_WhenNameIsExist_ReturnsNameAlreadyExistsError()
    {
        var request = new CourseRequest("Mathematics", "MATH101", 1);

        A.CallTo(() => _courseRepo.AnyAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .ReturnsNextFromSequence(true, false);

        var command = new AddCourseCommand(request.Name, request.Code, request.Level);

        var result = await _handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, CourseErrors.NameAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenCodeIsExist_ReturnsCodeAlreadyExistsError()
    {
        var request = new CourseRequest("Mathematics", "MATH101", 1);

        A.CallTo(() => _courseRepo.AnyAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .ReturnsNextFromSequence(false, true);

        var command = new AddCourseCommand(request.Name, request.Code, request.Level);

        var result = await _handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, CourseErrors.CodeAlreadyExists);
    }

    [Fact]
    public async Task Handle_PassValidData_ReturnsSuccess()
    {
        var request = new CourseRequest("Mathematics", "MATH101", 1);
        var course = Course.Create(request.Name, request.Code, request.Level);

        A.CallTo(() => _courseRepo.AnyAsync(A<Expression<Func<Course, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .ReturnsNextFromSequence(false, false);

        A.CallTo(() => _courseRepo.AddAsync(A<Course>.Ignored, A<CancellationToken>.Ignored))
            .Returns(course);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .Returns(1);

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.CompletedTask);

        var command = new AddCourseCommand(request.Name, request.Code, request.Level);

        var result = await _handler.Handle(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Name, course.Name);
        Assert.Equal(result.Value.Code, course.Code);
        Assert.Equal(result.Value.Level, course.Level);

        A.CallTo(() => _courseRepo.AddAsync(A<Course>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
