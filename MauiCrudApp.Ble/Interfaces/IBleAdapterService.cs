using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

namespace MauiCrudApp.Ble.Interfaces;

public interface IBleAdapterService
{
    bool IsScanning { get; }
    bool IsConnecting { get; }
    Task StartScanningAsync();
    Task StopScanningAsync();
    Task ConnectAsync(IDevice device);
    Task DisconnectAsync(IDevice device);
    Task ConnectToKnownDeviceAsync(Guid deviceId);
    Task CancelConnectingAsync();
    event EventHandler<DeviceEventArgs> DeviceConnected;
    event EventHandler<DeviceEventArgs> DeviceDisconnected;
    event EventHandler<DeviceErrorEventArgs> DeviceConnectionLost;
    event EventHandler<DeviceErrorEventArgs> DeviceConnectionError;
    event EventHandler<DeviceEventArgs> DeviceDiscovered;
    event EventHandler<bool> ScanStateChanged;
    event EventHandler<bool> ConnectStateChanged;
}
