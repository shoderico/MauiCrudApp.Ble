using Plugin.BLE.Abstractions.Contracts;

namespace MauiCrudApp.Ble.Interfaces;

public interface IBleCharacteristic
{
    Guid Id { get; }
    string Name { get; }
    ICharacteristic NativeCharacteristic { get; }
    bool CanRead { get; }
    bool CanWrite { get; }
    bool CanNotify { get; }
    bool IsNotifying { get; }
    byte[] Value { get; }

    event EventHandler<BleValueChangedEventArgs> ValueChanged;

    Task StartNotificationsAsync();
    Task StopNotificationsAsync();

    Task<byte[]> ReadAsync();
    Task WriteAsync(byte[] value);
}

public class BleValueChangedEventArgs : EventArgs
{
    public byte[] Value { get; }
    public BleValueChangeSource Source { get; }

    public BleValueChangedEventArgs(byte[] value, BleValueChangeSource source)
    {
        Value = value;
        Source = source;
    }
}

public enum BleValueChangeSource
{
    Read,
    Write,
    Notify
}
