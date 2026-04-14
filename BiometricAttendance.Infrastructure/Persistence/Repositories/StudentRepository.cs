namespace BiometricAttendance.Infrastructure.Persistence.Repositories;

public class StudentRepository(ApplicationDbContext context) : GenericRepositoryWithPagination<Student>(context), IStudentRepository
{
}
