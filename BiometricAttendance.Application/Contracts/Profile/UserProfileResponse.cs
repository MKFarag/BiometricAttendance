namespace BiometricAttendance.Application.Contracts.Profile;

// CHECK: Remove UserName if u want 

public record UserProfileResponse(
    string Id,
    string FirstName,
    string LastName,
    string UserName,
    string Email
);
