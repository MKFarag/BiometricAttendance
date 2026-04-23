namespace BiometricAttendance.Infrastructure.Fingerprint.SerialPort;

public interface ISerialPortService
{
    event Action<string> DataReceived;
    string LastReceivedData { get; }
    string LatestProcessedFingerprintId { get; }
    void Start();
    void Stop();
    void SendCommand(string command);
    void ResetLastScan();
}
