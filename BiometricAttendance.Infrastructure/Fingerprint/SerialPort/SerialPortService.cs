using System.IO.Ports;
using System.Text.RegularExpressions;

namespace BiometricAttendance.Infrastructure.Fingerprint.SerialPort;

public sealed partial class SerialPortService : ISerialPortService, IDisposable
{
    private string _latestProcessedFingerprintId = string.Empty;
    private string _lastReceivedData = string.Empty;
    private readonly System.IO.Ports.SerialPort _serialPort;
    private readonly ILogger<SerialPortService> _logger;

    public event Action<string> DataReceived = delegate { };

    public string LastReceivedData
    {
        get => _lastReceivedData;
        private set => _lastReceivedData = value;
    }

    public string LatestProcessedFingerprintId
    {
        get => _latestProcessedFingerprintId;
        private set => _latestProcessedFingerprintId = value;
    }

    [GeneratedRegex(@"#(\d+)")]
    private static partial Regex FingerprintIdRegex();

    public SerialPortService(string portName, int baudRate, ILogger<SerialPortService> logger)
    {
        _serialPort = new System.IO.Ports.SerialPort(portName, baudRate)
        {
            Parity = Parity.None,
            StopBits = StopBits.One,
            DataBits = 8,
            Handshake = Handshake.None,
            DtrEnable = true,
            RtsEnable = true
        };

        _serialPort.DataReceived += OnDataReceived;
        _logger = logger;
    }

    /// <summary>
    /// Opens the serial port connection to start receiving fingerprint data.
    /// Does nothing if the port is already open.
    /// </summary>
    public void Start()
    {
        if (!_serialPort.IsOpen)
            _serialPort.Open();
    }

    /// <summary>
    /// Closes the serial port connection and stops receiving data.
    /// Does nothing if the port is already closed.
    /// </summary>
    public void Stop()
    {
        if (_serialPort.IsOpen)
            _serialPort.Close();
    }

    /// <summary>
    /// Sends a command string to the fingerprint hardware via the serial port.
    /// Does nothing if the port is not open.
    /// </summary>
    public void SendCommand(string command)
    {
        if (_serialPort.IsOpen)
            _serialPort.WriteLine(command);
    }

    /// <summary>
    /// Resets the last received data and the latest processed fingerprint ID.
    /// Should be called after processing a fingerprint to prepare for the next scan.
    /// </summary>
    public void ResetLastScan()
    {
        LastReceivedData = string.Empty;
        LatestProcessedFingerprintId = string.Empty;
    }

    /// <summary>
    /// Event handler triggered when data arrives from the serial port.
    /// Reads the incoming line, processes it, and fires the DataReceived event.
    /// </summary>
    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        string data;

        try
        {
            data = _serialPort.ReadLine().Trim();
        }
        catch (IOException)
        {
            return;
        }
        catch (InvalidOperationException)
        {
            return;
        }

        if (string.IsNullOrEmpty(data))
            return;

        ProcessFingerprintData(data);
        LastReceivedData = data;
        DataReceived.Invoke(data);
    }

    /// <summary>
    /// Parses the raw data string received from the fingerprint hardware
    /// and routes it to the appropriate handler based on the message prefix.
    /// </summary>
    private void ProcessFingerprintData(string data)
    {
        if (data.StartsWith("MATCH: ID:")) { HandleMatch(data); return; }
        if (data.StartsWith("R-INFO")) { HandleRInfo(data); return; }
        if (data.StartsWith("INFO")) { HandleInfo(data); return; }
        if (data.StartsWith("ERROR")) { HandleError(data); return; }
        if (data.StartsWith("WARNING")) { HandleWarning(data); return; }
    }

    #region Handling

    /// <summary>
    /// Handles a successful fingerprint match message.
    /// Extracts and stores the fingerprint ID from the message.
    /// </summary>
    private void HandleMatch(string data)
    {
        var fingerprintId = data["MATCH: ID:".Length..].Trim();

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Extracted Fingerprint ID: {fid}", fingerprintId);

        LatestProcessedFingerprintId = fingerprintId;
    }

    /// <summary>
    /// Handles a registration info message from the hardware.
    /// Extracts the fingerprint ID using regex and stores it if found.
    /// </summary>
    private void HandleRInfo(string data)
    {
        var match = FingerprintIdRegex().Match(data);

        if (!match.Success)
            return;

        LatestProcessedFingerprintId = match.Groups[1].Value;

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Fingerprint: {data}", data);
    }

    /// <summary>
    /// Handles a general info message from the hardware and logs it.
    /// </summary>
    private void HandleInfo(string data)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Fingerprint: {data}", data["INFO: ".Length..]);
    }

    /// <summary>
    /// Handles an error message from the hardware and logs it.
    /// </summary>
    private void HandleError(string data)
    {
        if (_logger.IsEnabled(LogLevel.Error))
            _logger.LogError("Fingerprint: {data}", data["ERROR: ".Length..]);
    }

    /// <summary>
    /// Handles a warning message from the hardware and logs it.
    /// </summary>
    private void HandleWarning(string data)
    {
        if (_logger.IsEnabled(LogLevel.Warning))
            _logger.LogWarning("Fingerprint: {data}", data["WARNING: ".Length..]);
    }

    #endregion

    /// <summary>
    /// Releases all resources used by the SerialPortService.
    /// Closes the port, unsubscribes from events, and disposes the serial port.
    /// </summary>
    public void Dispose()
    {
        Stop();
        _serialPort.DataReceived -= OnDataReceived;
        _serialPort.Dispose();
        GC.SuppressFinalize(this);
    }
}
