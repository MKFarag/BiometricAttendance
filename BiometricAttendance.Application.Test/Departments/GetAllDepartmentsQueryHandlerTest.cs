using BiometricAttendance.Application.Features.Departments.GetAll;

namespace BiometricAttendance.Application.Test.Departments;

public class GetAllDepartmentsQueryHandlerTest : IClassFixture<MapsterTestFixture>
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IGenericRepository<Department> _departmentRepo = A.Fake<IGenericRepository<Department>>();
    private readonly ICacheService _cacheService = A.Fake<ICacheService>();
    private readonly GetAllDepartmentsQueryHandler _handler;

    public GetAllDepartmentsQueryHandlerTest(MapsterTestFixture _)
    {
        A.CallTo(() => _unitOfWork.Departments).Returns(_departmentRepo);
        _handler = new GetAllDepartmentsQueryHandler(_unitOfWork, _cacheService);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsDepartments()
    {
        // Arrange
        var departments = new List<Department>
        {
            new() { Id = 1, Name = "IT" },
            new() { Id = 2, Name = "CS" }
        };

        A.CallTo(() => _cacheService.GetOrCreateAsync(
            A<string>.Ignored,
            A<Func<CancellationToken, Task<IEnumerable<Department>>>>.Ignored,
            A<TimeSpan?>.Ignored,
            A<IEnumerable<string>?>.Ignored,
            A<CancellationToken>.Ignored))
            .Returns(departments);

        var query = new GetAllDepartmentsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(departments.Count, result.Count());
        Assert.Contains(result, x => x.Name == departments[0].Name);
        Assert.Contains(result, x => x.Name == departments[1].Name);

        A.CallTo(() => _cacheService.GetOrCreateAsync(
            A<string>.Ignored,
            A<Func<CancellationToken, Task<IEnumerable<Department>>>>.Ignored,
            A<TimeSpan?>.Ignored,
            A<IEnumerable<string>?>.Ignored,
            A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
