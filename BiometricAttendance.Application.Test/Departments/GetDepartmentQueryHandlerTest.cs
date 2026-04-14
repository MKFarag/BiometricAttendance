using BiometricAttendance.Application.Contracts.Departments;
using BiometricAttendance.Application.Features.Departments.Get;
using System.Linq.Expressions;

namespace BiometricAttendance.Application.Test.Departments;

public class GetDepartmentQueryHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IGenericRepository<Department> _departmentRepo = A.Fake<IGenericRepository<Department>>();
    private readonly IGenericRepository<DepartmentCourse> _departmentCourseRepo = A.Fake<IGenericRepository<DepartmentCourse>>();
    private readonly IStudentRepository _studentRepo = A.Fake<IStudentRepository>();
    private readonly GetDepartmentsQueryHandler _handler;

    public GetDepartmentQueryHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Departments).Returns(_departmentRepo);
        A.CallTo(() => _unitOfWork.DepartmentCourses).Returns(_departmentCourseRepo);
        A.CallTo(() => _unitOfWork.Students).Returns(_studentRepo);
        _handler = new GetDepartmentsQueryHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenDepartmentNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var department = new Department { Id = 1 };

        A.CallTo(() => _departmentRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns((Department?)null);

        var query = new GetDepartmentsQuery(department.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DepartmentErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_ValidId_ReturnsDepartment()
    {
        // Arrange
        var department = new Department { Id = 1, Name = "department" };
        var departmentDetail = new DepartmentDetailResponse(department.Id, department.Name, 3, 2);

        A.CallTo(() => _departmentRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(department);

        A.CallTo(() => _studentRepo.CountAsync(A<Expression<Func<Student, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(departmentDetail.StudentsCount);

        A.CallTo(() => _departmentCourseRepo.CountAsync(A<Expression<Func<DepartmentCourse, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(departmentDetail.CoursesCount);

        var query = new GetDepartmentsQuery(department.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Id, department.Id);
        Assert.Equal(result.Value.StudentsCount, departmentDetail.StudentsCount);
        Assert.Equal(result.Value.CoursesCount, departmentDetail.CoursesCount);
    }
}
