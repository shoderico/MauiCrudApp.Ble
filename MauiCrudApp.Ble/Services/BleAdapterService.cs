using CommunityToolkit.Mvvm.ComponentModel;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

using MauiCrudApp.Ble.Interfaces;

namespace MauiCrudApp.Ble.Services;

public partial class BleAdapterService : ObservableObject, IBleAdapterService
{
    [ObservableProperty]
    private bool isScanning;

    [ObservableProperty]
    private bool isConnecting;


    public event EventHandler<DeviceEventArgs> DeviceConnected;
    public event EventHandler<DeviceEventArgs> DeviceDisconnected;
    public event EventHandler<DeviceErrorEventArgs> DeviceConnectionLost;
    public event EventHandler<DeviceErrorEventArgs> DeviceConnectionError;
    public event EventHandler<DeviceEventArgs> DeviceDiscovered;
    public event EventHandler<bool> ScanStateChanged;
    public event EventHandler<bool> ConnectStateChanged;


    private readonly IAdapter _adapter;
    private CancellationTokenSource _scanCts;
    private CancellationTokenSource _connectCts;

    public BleAdapterService()
    {
        _adapter = CrossBluetoothLE.Current.Adapter;

        _adapter.DeviceDiscovered += (s, e) => DeviceDiscovered?.Invoke(this, e);
        _adapter.DeviceConnected += (s, e) => DeviceConnected?.Invoke(this, e);
        _adapter.DeviceDisconnected += (s, e) => DeviceDisconnected?.Invoke(this, e);
        _adapter.DeviceConnectionLost += (s, e) => DeviceConnectionLost?.Invoke(this, e);
        _adapter.DeviceConnectionError += (s, e) => DeviceConnectionError?.Invoke(this, new DeviceErrorEventArgs() { Device = e.Device, ErrorMessage = e.ErrorMessage });

        _adapter.ScanTimeoutElapsed += OnScanTimeoutElapsed;
    }


    private void OnScanTimeoutElapsed(object? sender, EventArgs e)
    {
        // does not fire sometimes?
        IsScanning = false;
    }


    public async Task StartScanningAsync()
    {
        if (IsScanning)
            await StopScanningAsync();

        try
        {
            _adapter.ScanTimeoutElapsed -= OnScanTimeoutElapsed;
            _adapter.ScanTimeoutElapsed += OnScanTimeoutElapsed;
            _adapter.ScanMode = ScanMode.LowLatency;

            _scanCts = new CancellationTokenSource();
            int timeout = _adapter.ScanTimeout > 0 ? _adapter.ScanTimeout : 30000; // default 30 sec.
            _scanCts.CancelAfter(timeout);

            IsScanning = true;
            await _adapter.StartScanningForDevicesAsync(cancellationToken: _scanCts.Token);
            IsScanning = false;
        }
        catch (OperationCanceledException)
        {
            IsScanning = false;
        }
        catch (Exception)
        {
            IsScanning = false;
            throw;
        }
        finally
        {
            _scanCts?.Dispose();
            _scanCts = null;
        }
    }


    public async Task StopScanningAsync()
    {
        try
        {
            if (IsScanning)
            {
                _scanCts?.Cancel();
                if (_adapter.IsScanning)
                    await _adapter.StopScanningForDevicesAsync();
            }
        }
        finally
        {
            IsScanning = false;
            _scanCts?.Dispose();
            _scanCts = null;
        }
    }


    public async Task ConnectAsync(IDevice device)
    {
        if (device == null)
            throw new ArgumentNullException(nameof(device));

        if (device.State == Plugin.BLE.Abstractions.DeviceState.Connected)
            return;

        try
        {
            _connectCts = new CancellationTokenSource();
            int timeout = 30000; // 30 seconds
            _connectCts.CancelAfter(timeout);

            IsConnecting = true;

            await _adapter.ConnectToDeviceAsync(device, cancellationToken: _connectCts.Token);

            IsConnecting = false;
        }
        catch (OperationCanceledException)
        {
            IsConnecting = false;
        }
        catch (Exception)
        {
            IsConnecting = false;
            throw;
        }
        finally
        {
            _connectCts?.Dispose();
            _connectCts = null;
        }
    }


    public async Task ConnectToKnownDeviceAsync(Guid deviceId)
    {
        if (deviceId == Guid.Empty)
            throw new ArgumentException("Invalid device ID.", nameof(deviceId));

        try
        {
            _connectCts = new CancellationTokenSource();
            int timeout = 30000; // 30 seconds
            _connectCts.CancelAfter(timeout);

            IsConnecting = true;

            await _adapter.ConnectToKnownDeviceAsync(deviceId, cancellationToken: _connectCts.Token);

            IsConnecting = false;
        }
        catch (OperationCanceledException)
        {
            IsConnecting = false;
            throw;
        }
        catch (Exception)
        {
            IsConnecting = false;
            throw;
        }
        finally
        {
            _connectCts?.Dispose();
            _connectCts = null;
        }
    }

    public async Task DisconnectAsync(IDevice device)
    {
        if (device == null)
            throw new ArgumentNullException(nameof(device));

        await _adapter.DisconnectDeviceAsync(device);
    }


    public Task CancelConnectingAsync()
    {
        try
        {
            if (_connectCts != null)
            {
                _connectCts?.Cancel();
                DeviceConnectionError?.Invoke(this, new DeviceErrorEventArgs() { Device = null, ErrorMessage = "onnection was canceled by the user." });
            }

            IsConnecting = false;

        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            _connectCts?.Dispose();
            _connectCts = null;
        }

        return Task.CompletedTask;
    }


    partial void OnIsConnectingChanged(bool value)
    {
        ConnectStateChanged?.Invoke(this, value);
    }

    partial void OnIsScanningChanged(bool value)
    {
        ScanStateChanged?.Invoke(this, value);
    }
}
