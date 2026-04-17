using BiometricAttendance.Application.Features.Students.AddCourses;

namespace BiometricAttendance.Application.Test.Students;

public class AddStudentCoursesCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IStudentRepository _studentRepo = A.Fake<IStudentRepository>();
    private readonly IGenericRepository<StudentCourse> _studentCoursesRepo = A.Fake<IGenericRepository<StudentCourse>>();
    private readonly IGenericRepository<DepartmentCourse> _departmentCoursesRepo = A.Fake<IGenericRepository<DepartmentCourse>>();
    private readonly AddStudentCoursesCommandHandler _handler;

    public AddStudentCoursesCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Students).Returns(_studentRepo);
        A.CallTo(() => _unitOfWork.StudentCourses).Returns(_studentCoursesRepo);
        A.CallTo(() => _unitOfWork.DepartmentCourses).Returns(_departmentCoursesRepo);
        _handler = new AddStudentCoursesCommandHandler(_unitOfWork);
    }
}
