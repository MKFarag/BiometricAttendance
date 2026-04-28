namespace BiometricAttendance.Application.Features.Instructors.GetAll;

public record GetAllInstructorsQuery : IRequest<IEnumerable<InstructorResponse>>;
