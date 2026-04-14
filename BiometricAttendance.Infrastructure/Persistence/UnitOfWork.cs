using BiometricAttendance.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace BiometricAttendance.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed = false;

    public IGenericRepository<Course> Courses { get; private set; }
    public IGenericRepository<Department> Departments { get; private set; }
    public IGenericRepository<DepartmentCourse> DepartmentCourses { get; private set; }
    public IGenericRepository<InstructorPass> InstructorPasses { get; private set; }
    public IRoleRepository Roles { get; private set; }
    public IStudentRepository Students { get; private set; }
    public IUserRepository Users { get; private set; }


    public UnitOfWork(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _context = context;

        Courses = new GenericRepository<Course>(_context);
        Departments = new GenericRepository<Department>(_context);
        DepartmentCourses = new GenericRepository<DepartmentCourse>(_context);
        InstructorPasses = new GenericRepository<InstructorPass>(_context);
        Users = new UserRepository(_context, userManager);
        Students = new StudentRepository(_context);
        Roles = new RoleRepository(_context, roleManager);
    }

    public int Complete()
        => _context.SaveChanges();

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
            return;

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
            throw new InvalidOperationException("No active transaction to commit.");

        await _currentTransaction.CommitAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public async Task RollbackTransactionAsync()
    {
        if (_currentTransaction is null)
            return;

        await _currentTransaction.RollbackAsync(CancellationToken.None);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _currentTransaction?.Dispose();
                _context?.Dispose();
            }

            _disposed = true;
        }
    }
}
