using BiometricAttendance.Application.Contracts.Courses;
using BiometricAttendance.Application.Contracts.Departments;
using BiometricAttendance.Application.Features.Courses.Get;
using Castle.Components.DictionaryAdapter;

namespace BiometricAttendance.Application.Test.Courses;

public class GetCourseQueryHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IGenericRepository<Course> _courseRepo = A.Fake<IGenericRepository<Course>>();
    private readonly IGenericRepository<Department> _departmentRepo = A.Fake<IGenericRepository<Department>>();
    private readonly GetCourseQueryHandler _handler;

    public GetCourseQueryHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Courses).Returns(_courseRepo);
        A.CallTo(() => _unitOfWork.Departments).Returns(_departmentRepo);
        _handler = new GetCourseQueryHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenCourseNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var courseId = 1;

        A.CallTo(() => _courseRepo.FindAsync(A<Expression<Func<Course, bool>>>.Ignored, A<string[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns((Course?)null);

        var query = new GetCourseQuery(courseId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(CourseErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_ValidId_ReturnsCourse()
    {
        // Arrange
        var course = Course.Create("Math", "MATH101", 1);

        A.CallTo(() => _courseRepo.FindAsync(A<Expression<Func<Course, bool>>>.Ignored, A<string[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(course);

        var query = new GetCourseQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Name, course.Name);
        Assert.Equal(result.Value.Code, course.Code);
    }
}
