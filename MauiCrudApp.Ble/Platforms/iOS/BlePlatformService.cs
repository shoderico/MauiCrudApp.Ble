using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

using MauiCrudApp.Common.Interfaces;
using MauiCrudApp.Ble.Services;

namespace MauiCrudApp.Ble.Platforms;

public class BlePlatformService : BlePlatformServiceBase
{
    private IDialogService _dialogService;
    private IBluetoothLE _ble;

    public BlePlatformService(IDialogService dialogService)
    {
        _dialogService = dialogService;
        _ble = CrossBluetoothLE.Current;
        _ble.StateChanged += OnStateChanged;
    }

    private async void OnStateChanged(object? sender, Plugin.BLE.Abstractions.EventArgs.BluetoothStateChangedArgs e)
    {
        try
        {
            // On iOS, IBluetoothLE.State returns BluetoothState.Unknown until IAdapter executes the first scan.
            // Therefore, checking IBluetoothLE.State in the CheckBluetoothPermissionAsync does not work.
            // So, we catch the StateChanged event and show the message.
            switch (e.NewState)
            {
                case BluetoothState.Unavailable:
                case BluetoothState.Unauthorized:
                case BluetoothState.Off:
                    Console.WriteLine("Bluetooth is off, or Bluetooth permission is denied for this app");
                    if (await _dialogService.DisplayAlert(
                        "Cannot use Bluetooth",
                        "Bluetooth is either turned off or the app's Bluetooth permission is not granted. Please turn on Bluetooth and grant Bluetooth permission from the app's settings." + $" State: {_ble.State}",
                        "Go to app settings",
                        "Cancel"
                    ))
                    {
                        AppInfo.ShowSettingsUI();
                    }
                    return;
            }
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlert("Error", $"An error occurred while checking Bluetooth permission: {ex.Message}", "OK");
            return;
        }
    }

    public override Task<bool> CheckBluetoothPermissionAsync()
    {
        // iOS does not need the Permission of Location Service to use Bluetooth.

        return Task.FromResult(true);
    }
}
