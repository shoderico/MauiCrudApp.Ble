using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

using MauiCrudApp.Ble.Interfaces;
using MauiCrudApp.Ble.Models;
using System.Runtime.CompilerServices;

namespace MauiCrudApp.Ble.Services;

public class BleDeviceManager : IBleDeviceManager
{
    private readonly IBleAdapterService _adapterService;
    private readonly IBleDevice _bleDevice;
    private CancellationTokenSource _discoverCts;

    private const string LAST_DEVICE_ID_KEY = "LastDeviceId";
    private const string LAST_DEVICE_NAME_KEY = "LastDeviceName";


    public BleDeviceManager(IBleAdapterService adapterService)
    {
        _adapterService = adapterService;

        
        var lastDeviceId = Preferences.Get(LAST_DEVICE_ID_KEY, Guid.Empty.ToString());
        var lastDeviceName = Preferences.Get(LAST_DEVICE_NAME_KEY, "Unknown");

        var lastDeviceGuid = Guid.TryParse(lastDeviceId, out var guid) ? guid : Guid.Empty;
        _bleDevice = new BleDevice(lastDeviceGuid, lastDeviceName);


        _adapterService.DeviceConnected += OnDeviceConnected;
        _adapterService.DeviceDisconnected += OnDeviceDisconnected;
        _adapterService.DeviceConnectionLost += OnDeviceConnectionLost;
        _adapterService.DeviceConnectionError += OnDeviceConnectionError;
    }



    public IBleDevice BleDevice => _bleDevice;

    public async Task ConnectAsync()
    {
        if (_bleDevice.Id == Guid.Empty)
        {
            _bleDevice.ConnectionState = BleDeviceConnectionState.Unknown;
            _bleDevice.ConnectionError = "No device selected.";
            return;
        }

        _bleDevice.ConnectionState = BleDeviceConnectionState.Connecting;
        _bleDevice.ConnectionError = "";
        try
        {
            await _adapterService.ConnectToKnownDeviceAsync(_bleDevice.Id);
        }
        catch (OperationCanceledException)
        {
            _bleDevice.ConnectionState = BleDeviceConnectionState.Disconnected;
            _bleDevice.ConnectionError = "Connection was canceled.";
        }
        catch (Exception ex)
        {
            _bleDevice.ConnectionState = BleDeviceConnectionState.Error;
            _bleDevice.ConnectionError = ex.Message;
        }
    }

    public async Task DisconnectAsync()
    {
        if (_bleDevice.Id == Guid.Empty || _bleDevice.NativeDevice == null)
        {
            _bleDevice.ConnectionState = BleDeviceConnectionState.Unknown;
            _bleDevice.ConnectionError = "No device to disconnect.";
            await CleanupServicesAsync();
            return;
        }

        try
        {
            // Perform cleanup
            await CleanupServicesAsync();

            // Disconnect
            await _adapterService.DisconnectAsync(_bleDevice.NativeDevice);
            _bleDevice.ConnectionState = BleDeviceConnectionState.Disconnected;
            _bleDevice.ConnectionError = null;
        }
        catch (Exception ex)
        {
            _bleDevice.ConnectionState = BleDeviceConnectionState.Error;
            _bleDevice.ConnectionError = ex.Message;
        }
    }

    public async Task ReconnectAsync(Guid deviceId)
    {
        if (deviceId == Guid.Empty)
        {
            _bleDevice.ConnectionState = BleDeviceConnectionState.Unknown;
            _bleDevice.ConnectionError = "Invalid device ID.";
            await CleanupServicesAsync();
            return;
        }

        _bleDevice.ConnectionState = BleDeviceConnectionState.Connecting;
        _bleDevice.Id = deviceId;
        _bleDevice.Name = "Unknown";
        try
        {
            await _adapterService.ConnectToKnownDeviceAsync(deviceId);
        }
        catch (Exception ex)
        {
            _bleDevice.ConnectionState = BleDeviceConnectionState.Error;
            _bleDevice.ConnectionError = ex.Message;
        }
    }

    public async Task SelectDeviceAsync(IDevice device)
    {
        if (device == null)
            throw new ArgumentNullException(nameof(device));

        if (_bleDevice.NativeDevice != null && _bleDevice.ConnectionState == BleDeviceConnectionState.Connected)
        {
            await _adapterService.DisconnectAsync(_bleDevice.NativeDevice);
        }


        _bleDevice.UpdateFrom(device);
        _bleDevice.ConnectionState = BleDeviceConnectionState.Disconnected;
        _bleDevice.ConnectionError = null;


        Preferences.Set(LAST_DEVICE_ID_KEY, _bleDevice.Id.ToString());
        Preferences.Set(LAST_DEVICE_NAME_KEY, _bleDevice.Name);
    }

    public async Task ResetDeviceAsync()
    {
        _bleDevice.Reset();
        await CleanupServicesAsync();
    }



    public async Task CancelConnectingAsync()
    {
        if (_bleDevice.ConnectionState == BleDeviceConnectionState.Connecting
            || _bleDevice.ConnectionState == BleDeviceConnectionState.Discovering)
        {
            try
            {
                _discoverCts?.Cancel();
                await _adapterService.CancelConnectingAsync();
                await CleanupServicesAsync();
                _bleDevice.ConnectionState = BleDeviceConnectionState.Disconnected;
                _bleDevice.ConnectionError = "Connection or discovery was canceled by the user.";
            }
            finally
            {
                _discoverCts?.Dispose();
                _discoverCts = null;
            }
        }
    }


    private async void OnDeviceConnected(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
    {
        if (e.Device.Id == _bleDevice.Id)
        {
            _bleDevice.UpdateFrom(e.Device);
            _bleDevice.ConnectionState = BleDeviceConnectionState.Discovering;
            _bleDevice.ConnectionError = null;

            Preferences.Set(LAST_DEVICE_ID_KEY, _bleDevice.Id.ToString());
            Preferences.Set(LAST_DEVICE_NAME_KEY, _bleDevice.Name);

            try
            {
                _discoverCts = new CancellationTokenSource();
                _discoverCts.CancelAfter(30000);

                await DiscoverServicesAsync(_discoverCts.Token);
                _bleDevice.ConnectionState = BleDeviceConnectionState.Connected;
            }
            catch (OperationCanceledException)
            {
                // treated cancellation in DiscoverServicesAsync
            }
            catch (Exception ex)
            {
                _bleDevice.ConnectionState = BleDeviceConnectionState.Error;
                _bleDevice.ConnectionError = $"Service discovery failed: {ex.Message}";
            }
            finally
            {
                _discoverCts?.Dispose();
                _discoverCts = null;
            }
        }
    }

    private void OnDeviceDisconnected(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
    {
        if (e.Device.Id == _bleDevice.Id)
        {
            try
            {
                CleanupServicesAsync().GetAwaiter().GetResult();
                _bleDevice.ConnectionState = BleDeviceConnectionState.Disconnected;
                _bleDevice.ConnectionError = null;
            }
            catch (Exception ex)
            {
                _bleDevice.ConnectionState = BleDeviceConnectionState.Error;
                _bleDevice.ConnectionError = $"Disconnection handling failed: {ex.Message}";
            }
        }
    }

    private void OnDeviceConnectionLost(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceErrorEventArgs e)
    {
        if (e.Device.Id == _bleDevice.Id)
        {
            try
            {
                CleanupServicesAsync().GetAwaiter().GetResult();
                _bleDevice.ConnectionState = BleDeviceConnectionState.Disconnected;
                _bleDevice.ConnectionError = e.ErrorMessage;
            }
            catch (Exception ex)
            {
                _bleDevice.ConnectionState = BleDeviceConnectionState.Error;
                _bleDevice.ConnectionError = $"Connection lost handling failed: {ex.Message}";
            }
        }
    }

    private void OnDeviceConnectionError(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceErrorEventArgs e)
    {
        if (e.Device == null || e.Device.Id == _bleDevice.Id)
        {
            try
            {
                CleanupServicesAsync().GetAwaiter().GetResult();
                _bleDevice.ConnectionState = BleDeviceConnectionState.Error;
                _bleDevice.ConnectionError = e.ErrorMessage;
            }
            catch (Exception ex)
            {
                _bleDevice.ConnectionState = BleDeviceConnectionState.Error;
                _bleDevice.ConnectionError = $"Connection error handling failed: {ex.Message}";
            }
        }
    }





    #region Services / Characteristics

    // Discover services and characteristics
    private async Task DiscoverServicesAsync(CancellationToken cancellationToken)
    {
        try
        {
            _bleDevice.ClearServices();
            _bleDevice.ConnectionState = BleDeviceConnectionState.Discovering;

            var services = await _bleDevice.NativeDevice.GetServicesAsync(cancellationToken);
            foreach (var service in services)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var bleService = new BleService(service.Id, service.Name, service);
                var characteristics = await service.GetCharacteristicsAsync();
                foreach (var characteristic in characteristics)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var bleCharacteristic = new BleCharacteristic(characteristic.Id, characteristic.Name, characteristic);
                    bleService.AddCharacteristic(bleCharacteristic);

                }
                _bleDevice.AddService(bleService);
            }
        }
        catch (OperationCanceledException)
        {
            _bleDevice.ConnectionState = BleDeviceConnectionState.Disconnected;
            _bleDevice.ConnectionError = "Service discovery was canceled.";
            throw;
        }
        catch (Exception ex)
        {
            _bleDevice.ConnectionState = BleDeviceConnectionState.Error;
            _bleDevice.ConnectionError = $"Service discovery failed: {ex.Message}";
            throw;
        }
    }


    private async Task CleanupServicesAsync()
    {
        try
        {
            // Safety check: No device or invalid state
            if (_bleDevice.Id == Guid.Empty || _bleDevice.ConnectionState == BleDeviceConnectionState.Unknown)
            {
                _bleDevice.ClearServices();
                return;
            }

            // Stop notifications for all characteristics
            foreach (var bleService in _bleDevice.Services.ToList())
            {
                foreach (var bleCharacteristic in bleService.Characteristics.Where(c => c.IsNotifying).ToList())
                {
                    try
                    {
                        // Stop notification if still active
                        if (bleCharacteristic.NativeCharacteristic != null)
                        {
                            await bleCharacteristic.StopNotificationsAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        _bleDevice.ConnectionError = $"Failed to stop notifications for characteristic {bleCharacteristic.Id}: {ex.Message}";
                        // Continue processing other characteristics
                    }
                }
            }

            // Clear all services and characteristics
            _bleDevice.ClearServices();
        }
        catch (Exception ex)
        {
            _bleDevice.ConnectionState = BleDeviceConnectionState.Error;
            _bleDevice.ConnectionError = $"Device cleanup failed: {ex.Message}";
        }
    }


    #endregion
}


