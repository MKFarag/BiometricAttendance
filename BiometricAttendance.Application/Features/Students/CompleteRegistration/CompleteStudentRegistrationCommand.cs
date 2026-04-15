namespace BiometricAttendance.Application.Features.Students.CompleteRegistration;

public record CompleteStudentRegistrationCommand(string UserId, CompleteStudentRegistrationRequest Request) : IRequest<Result>;
