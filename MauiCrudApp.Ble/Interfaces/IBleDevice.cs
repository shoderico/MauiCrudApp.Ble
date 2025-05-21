using Plugin.BLE.Abstractions.Contracts;

namespace MauiCrudApp.Ble.Interfaces;

public enum BleDeviceConnectionState { Disconnected, Connecting, Discovering, Connected, Error, Unknown }

public interface IBleDevice
{
    Guid Id { get; set; }
    string Name { get; set; }
    BleDeviceConnectionState ConnectionState { get; set; }
    string ConnectionError { get; set; }
    IDevice NativeDevice { get; }
    IReadOnlyList<IBleService> Services { get; }

    void UpdateFrom(IDevice device);
    void Reset();
    void AddService(IBleService bleService);
    void ClearServices();

    event EventHandler ServicesChanged;

    bool CanConnect { get; }
    bool CanDisconnect { get; }
    bool CanCancelConnecting { get; }
    bool CanReset { get; }

}
