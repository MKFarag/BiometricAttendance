using BiometricAttendance.Application.Features.Courses.GetAll;

namespace BiometricAttendance.Application.Test.Courses;

public class GetAllCoursesQueryHandlerTest : IClassFixture<MapsterTestFixture>
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IGenericRepository<Course> _courseRepo = A.Fake<IGenericRepository<Course>>();
    private readonly ICacheService _cacheService = A.Fake<ICacheService>();
    private readonly GetAllCoursesQueryHandler _handler;

    public GetAllCoursesQueryHandlerTest(MapsterTestFixture _)
    {
        A.CallTo(() => _unitOfWork.Courses).Returns(_courseRepo);
        _handler = new GetAllCoursesQueryHandler(_unitOfWork, _cacheService);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsCourses()
    {
        var courses = new List<Course>
        {
            Course.Create("Math", "MATH101", 1),
            Course.Create("Physics", "PHY201", 2)
        };

        A.CallTo(() => _cacheService.GetOrCreateAsync(
            A<string>.Ignored,
            A<Func<CancellationToken, Task<IEnumerable<Course>>>>.Ignored,
            A<TimeSpan?>.Ignored,
            A<IEnumerable<string>?>.Ignored,
            A<CancellationToken>.Ignored))
            .Returns(courses);

        var query = new GetAllCoursesQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(courses.Count, result.Count());
        Assert.Contains(result, x => x.Name == courses[0].Name);
        Assert.Contains(result, x => x.Name == courses[1].Name);

        A.CallTo(() => _cacheService.GetOrCreateAsync(
            A<string>.Ignored,
            A<Func<CancellationToken, Task<IEnumerable<Course>>>>.Ignored,
            A<TimeSpan?>.Ignored,
            A<IEnumerable<string>?>.Ignored,
            A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
