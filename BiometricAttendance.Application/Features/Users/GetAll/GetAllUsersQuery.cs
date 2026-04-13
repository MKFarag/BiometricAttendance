namespace BiometricAttendance.Application.Features.Users.GetAll;

public record GetAllUsersQuery : IRequest<IEnumerable<UserResponse>>;
