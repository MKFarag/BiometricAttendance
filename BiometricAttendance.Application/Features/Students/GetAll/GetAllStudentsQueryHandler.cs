namespace BiometricAttendance.Application.Features.Students.GetAll;

public class GetAllStudentsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllStudentsQuery, IPaginatedList<StudentResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    private static readonly HashSet<string> _allowedSearchColumns = new(StringComparer.OrdinalIgnoreCase)
    { nameof(Student.Level), nameof(Student.Department)};

    public Task<IPaginatedList<StudentResponse>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken = default)
    {
        var filters = request.Filters.Check(_allowedSearchColumns);

        ColumnType searchColumnType = filters.SearchColumn switch
        {
            nameof(Student.Level) => ColumnType.Int,
            nameof(Student.Department) => ColumnType.String,
            _ => ColumnType.None,
        };

        var response = _unitOfWork.Students.GetPaginatedListAsync<StudentResponse>
            (filters.PageNumber, filters.PageSize, nameof(Student.Id), filters.SearchValue, filters.SearchColumn, searchColumnType, cancellationToken);

        return response;
    }
}
