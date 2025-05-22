using CommunityToolkit.Mvvm.ComponentModel;
using Plugin.BLE.Abstractions.Contracts;

using MauiCrudApp.Ble.Interfaces;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions;

namespace MauiCrudApp.Ble.Models;

public partial class BleCharacteristic : ObservableObject, IBleCharacteristic
{
    [ObservableProperty]
    private Guid id;

    [ObservableProperty]
    private string name;
    public ICharacteristic NativeCharacteristic { get; private set; }

    [ObservableProperty]
    private bool canRead;

    [ObservableProperty]
    private bool canWrite;

    [ObservableProperty]
    private bool canNotify;

    [ObservableProperty]
    private bool isNotifying;

    [ObservableProperty]
    private CharacteristicWriteType writeType;

    public byte[] Value { get; private set; }

    public event EventHandler<BleValueChangedEventArgs> ValueChanged;

    public BleCharacteristic(Guid id, string name, ICharacteristic nativeCharacteristic)
    {
        Id = id;
        Name = name ?? "Unknown Characteristic";
        NativeCharacteristic = nativeCharacteristic;
        CanRead = nativeCharacteristic.CanRead;
        CanWrite = nativeCharacteristic.CanWrite;
        CanNotify = nativeCharacteristic.CanUpdate;
        WriteType = nativeCharacteristic.WriteType;
        Value = Array.Empty<byte>();
        IsNotifying = false;
    }

    private void UpdateValue(byte[] value)
    {
        if (value != null)
        {
            Value = value;
            ValueChanged?.Invoke(this, new BleValueChangedEventArgs(value, BleValueChangeSource.Notify));
        }
    }

    public async Task StartNotificationsAsync()
    {
        if (IsNotifying == true)
            return;

        IsNotifying = true;
        await NativeCharacteristic.StartUpdatesAsync();
        NativeCharacteristic.ValueUpdated += OnValueUpdated;
    }

    public async Task StopNotificationsAsync()
    {
        if (IsNotifying == false)
            return;

        IsNotifying = false;
        await NativeCharacteristic.StopUpdatesAsync();
        NativeCharacteristic.ValueUpdated -= OnValueUpdated;
    }


    private void OnValueUpdated(object? sender, CharacteristicUpdatedEventArgs e)
    {
        UpdateValue(e.Characteristic.Value);
    }

    public async Task<byte[]> ReadAsync()
    {
        if (!CanRead)
            throw new InvalidOperationException("Characteristic is not readable.");

        try
        {
            (var value, int resultCode) = await NativeCharacteristic.ReadAsync();
            Value = value;
            ValueChanged?.Invoke(this, new BleValueChangedEventArgs(value, BleValueChangeSource.Read));
            return value;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Read failed: {ex.Message}", ex);
        }
    }

    public async Task WriteAsync(byte[] value)
    {
        if (!CanWrite)
            throw new InvalidOperationException("Characteristic is not writable.");
        if (value == null || value.Length > 10)
            throw new ArgumentException("Value must be 10 bytes or less.");

        try
        {
            await NativeCharacteristic.WriteAsync(value);
            Value = value;
            ValueChanged?.Invoke(this, new BleValueChangedEventArgs(value, BleValueChangeSource.Write));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Write failed: {ex.Message}", ex);
        }
    }

    partial void OnWriteTypeChanged(CharacteristicWriteType value)
    {
        if (NativeCharacteristic.Properties.HasFlag(value))
        {
            NativeCharacteristic.WriteType = value;
        }
        else
        {
            Console.WriteLine("not supported");
            throw new NotSupportedException("The specified write type is not supported by this characteristic.");
        }
    }
}
