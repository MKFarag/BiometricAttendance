namespace BiometricAttendance.Application.Interfaces;

public interface IUrlEncoder
{
    string Encode(string value);
    string? Decode(string encodedValue);
}
