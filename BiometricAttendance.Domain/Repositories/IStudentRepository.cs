namespace BiometricAttendance.Domain.Repositories;

public interface IStudentRepository : IGenericRepository<Student>
{
    /// <summary>Get paginated list of entities</summary>
    /// <typeparam name="TProjection">The type to project the entities to (e.g. StudentResponse)</typeparam>
    /// <param name="pageNumber">The page number to retrieve (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="primaryKey">The primary key column name</param>
    /// <param name="searchValue">Optional search value to filter results</param>
    /// <param name="searchColumn">Column name to search in</param>
    /// <param name="searchColumnType">Defines the data type of the search column (e.g., String, Int, None)</param>
    /// <returns>Paginated list of projected entities</returns>
    Task<IPaginatedList<TProjection>> GetPaginatedListAsync<TProjection>(
        int pageNumber, int pageSize, string primaryKey, string? searchValue, string? searchColumn,
        ColumnType searchColumnType, CancellationToken cancellationToken = default) where TProjection : class;

    Task<IEnumerable<Student>> FindAllWithNameAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default);
}
