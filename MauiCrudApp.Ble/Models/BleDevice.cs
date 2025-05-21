using CommunityToolkit.Mvvm.ComponentModel;
using Plugin.BLE.Abstractions.Contracts;
using System.Collections.ObjectModel;

using MauiCrudApp.Ble.Interfaces;

namespace MauiCrudApp.Ble.Models;


public partial class BleDevice : ObservableObject, IBleDevice
{
    [ObservableProperty]
    private Guid id = Guid.Empty;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private BleDeviceConnectionState connectionState = BleDeviceConnectionState.Disconnected;

    [ObservableProperty]
    private string connectionError = null;


    private IDevice? _device = null;


    private readonly ObservableCollection<IBleService> _services = new();
    public IReadOnlyList<IBleService> Services => _services;
    public event EventHandler ServicesChanged;


    public BleDevice()
    {
        UpdateCommandStates();
    }

    public BleDevice(IDevice device)
    {
        _device = device;
        Id = device.Id;
        Name = device.Name;
        ConnectionState = BleDeviceConnectionState.Disconnected;
        UpdateCommandStates();
    }

    public BleDevice(Guid id, string name = "Unknown")
    {
        Id = id;
        Name = name;
        ConnectionState = id == Guid.Empty ? BleDeviceConnectionState.Unknown : BleDeviceConnectionState.Disconnected;
        UpdateCommandStates();
    }

    public IDevice NativeDevice => _device;

    public void UpdateFrom(IDevice device)
    {
        _device = device;
        Id = _device.Id;
        Name = _device.Name;
        UpdateCommandStates();
    }

    public void Reset()
    {
        _device = null;
        Id = Guid.Empty;
        Name = "Unknwon";
        ConnectionState = BleDeviceConnectionState.Unknown;
        UpdateCommandStates();
    }




    public void AddService(IBleService service)
    {
        _services.Add(service);
        ServicesChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ClearServices()
    {
        _services.Clear();
        ServicesChanged?.Invoke(this, EventArgs.Empty);
    }





    partial void OnConnectionStateChanged(BleDeviceConnectionState value)
    {
        UpdateCommandStates();
    }

    partial void OnIdChanged(Guid value)
    {
        UpdateCommandStates(); 
    }

    [ObservableProperty]
    private bool canConnect = false;

    [ObservableProperty]
    private bool canDisconnect = false;

    [ObservableProperty]
    private bool canCancelConnecting = false;

    [ObservableProperty]
    private bool canReset = false;

    private void UpdateCommandStates()
    {
        CanConnect 
            = Id != Guid.Empty
            && (ConnectionState == BleDeviceConnectionState.Disconnected
            ||  ConnectionState == BleDeviceConnectionState.Error);

        CanDisconnect
            = Id != Guid.Empty
            && (ConnectionState == BleDeviceConnectionState.Connected
            ||  ConnectionState == BleDeviceConnectionState.Error);

        CanCancelConnecting
            = Id != Guid.Empty
            && (ConnectionState == BleDeviceConnectionState.Connecting
            ||  ConnectionState == BleDeviceConnectionState.Discovering);

        CanReset
            = Id != Guid.Empty
            && (ConnectionState == BleDeviceConnectionState.Disconnected
            ||  ConnectionState == BleDeviceConnectionState.Error);
    }

}
