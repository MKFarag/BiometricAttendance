namespace BiometricAttendance.Application.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        #region UserResponse

        config.NewConfig<(User user, IList<string> roles), UserResponse>()
            .Map(dest => dest, src => src.user)
            .Map(dest => dest.Roles, src => src.roles);

        #endregion

        #region StudentDetailResponse

        config.NewConfig<(Student student, User user), StudentDetailResponse>()
            .Map(dest => dest, src => src.student)
            .Map(dest => dest, src => src.user)
            .Map(dest => dest.Department, src => src.student.Department)
            .Map(dest => dest.Courses, src => src.student.Courses.Select(x => x.Course));

        #endregion
    }
}
