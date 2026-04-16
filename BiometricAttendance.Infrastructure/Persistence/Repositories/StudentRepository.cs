using BiometricAttendance.Application.Contracts.Students;

namespace BiometricAttendance.Infrastructure.Persistence.Repositories;

public class StudentRepository(ApplicationDbContext context) : GenericRepository<Student>(context), IStudentRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IPaginatedList<TProjection>> GetPaginatedListAsync<TProjection>(
        int pageNumber, int pageSize, string primaryKey, string? searchValue, string? searchColumn,
        ColumnType searchColumnType, CancellationToken cancellationToken = default) where TProjection : class
    {
        var query = _context.Students.AsNoTracking().AsQueryable();

        query = query
            .ApplyIncludesSafely([nameof(Student.Department)])
            .ApplySearchFilter(searchValue, searchColumn, searchColumnType)
            .OrderBy(primaryKey, OrderBy.Ascending);

        var joinedQuery = query.Join
            (
            _context.Users,
            student => student.UserId,
            user => user.Id,
            (student, user) => new StudentWithUserDto(student.Id, $"{user.FirstName} {user.LastName}", user.Email!, student.Level, student.DepartmentName)
            );

        var paginatedList = await PaginatedList<StudentWithUserDto>.CreateAsync(joinedQuery, pageNumber, pageSize, cancellationToken);

        var response = PaginatedList<TProjection>.CopyWithNewItems(paginatedList.Items.Adapt<List<TProjection>>(), paginatedList);

        return response;
    }
}
