using BiometricAttendance.Application.Features.Students.EnrollCourses;

namespace BiometricAttendance.Application.Test.Students;

public class EnrollStudentCoursesCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IStudentRepository _studentRepo = A.Fake<IStudentRepository>();
    private readonly IGenericRepository<StudentCourse> _studentCoursesRepo = A.Fake<IGenericRepository<StudentCourse>>();
    private readonly IGenericRepository<DepartmentCourse> _departmentCoursesRepo = A.Fake<IGenericRepository<DepartmentCourse>>();
    private readonly EnrollStudentCoursesCommandHandler _handler;

    public EnrollStudentCoursesCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Students).Returns(_studentRepo);
        A.CallTo(() => _unitOfWork.StudentCourses).Returns(_studentCoursesRepo);
        A.CallTo(() => _unitOfWork.DepartmentCourses).Returns(_departmentCoursesRepo);
        _handler = new EnrollStudentCoursesCommandHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenStudentNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var command = new EnrollStudentCoursesCommand(Guid.CreateVersion7().ToString(), [1, 2]);

        A.CallTo(() => _studentRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns((Student?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenRequestContainsInvalidCourses_ReturnsInvalidCoursesError()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 2, 1);
        var command = new EnrollStudentCoursesCommand(student.UserId, [1, 2, 3]);

        A.CallTo(() => _studentRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(student);

        A.CallTo(() => _departmentCoursesRepo.FindAllProjectionAsync(
                A<Expression<Func<DepartmentCourse, bool>>>.Ignored,
                A<Expression<Func<DepartmentCourse, int>>>.Ignored,
                A<bool>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([1, 2]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(StudentErrors.InvalidCourses, result.Error);

        A.CallTo(() => _studentCoursesRepo.AddRangeAsync(A<IEnumerable<StudentCourse>>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ReturnsSuccess()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 2, 1);
        var command = new EnrollStudentCoursesCommand(student.UserId, [1, 2]);
        IEnumerable<StudentCourse> studentCourses = 
            [new StudentCourse { CourseId = 1, StudentId = student.Id }, new StudentCourse { CourseId = 2, StudentId = student.Id }];

        A.CallTo(() => _studentRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(student);

        A.CallTo(() => _departmentCoursesRepo.FindAllProjectionAsync(
                A<Expression<Func<DepartmentCourse, bool>>>.Ignored,
                A<Expression<Func<DepartmentCourse, int>>>.Ignored,
                A<bool>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([1, 2, 3]);

        A.CallTo(() => _studentCoursesRepo.AddRangeAsync(A<IEnumerable<StudentCourse>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(studentCourses);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .Returns(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        A.CallTo(() => _studentCoursesRepo.AddRangeAsync(A<IEnumerable<StudentCourse>>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
