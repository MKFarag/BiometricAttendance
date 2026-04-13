namespace BiometricAttendance.Application.Interfaces;

public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions);
    string? ValidateToken(string token);
}
