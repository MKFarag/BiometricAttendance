namespace BiometricAttendance.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    #region DbSet

    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<DepartmentCourse> DepartmentCourses { get; set; }
    public DbSet<Fingerprint> Fingerprints { get; set; }
    public DbSet<InstructorPass> InstructorPasses { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentCourse> StudentCourses { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        #region Change delete behavior

        var cascadeFk = builder.Model
            .GetEntityTypes()
            .SelectMany(entityType => entityType.GetForeignKeys())
            .Where(Fk => Fk.DeleteBehavior == DeleteBehavior.Cascade && !Fk.IsOwnership);

        foreach (var fk in cascadeFk)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        #endregion

        #region Allow Cascade Delete



        #endregion

        base.OnModelCreating(builder);
    }
}
