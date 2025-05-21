using Plugin.BLE.Abstractions.Contracts;

namespace MauiCrudApp.Ble.Interfaces;

public interface IBleDeviceManager
{
    IBleDevice BleDevice { get; }
    Task ConnectAsync();
    Task DisconnectAsync();
    Task ReconnectAsync(Guid deviceId);
    Task SelectDeviceAsync(IDevice device);
    Task ResetDeviceAsync();
    Task CancelConnectingAsync();

    //Task StartNotificationsAsync(IBleCharacteristic bleCharacteristic);
    //Task StopNotificationsAsync(Guid serviceId, Guid characteristicId);
    //Task<byte[]> ReadCharacteristicAsync(Guid serviceId, Guid characteristicId);
    //Task WriteCharacteristicAsync(Guid serviceId, Guid characteristicId, byte[] value);
}
