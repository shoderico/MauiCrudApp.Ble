﻿using Android.Content;
using Android.Locations;
using Android.OS;
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
    }

    public async override Task<bool> CheckBluetoothPermissionAsync()
    {
        try
        {
            switch (_ble.State)
            {
                case BluetoothState.TurningOn:
                    Console.WriteLine("Bluetooth is TurningOn");
                    break;

                case BluetoothState.On:
                    Console.WriteLine("Bluetooth is ON");
                    break;

                case BluetoothState.Off:
                    Console.WriteLine("Bluetooth is OFF");
                    if (await _dialogService.DisplayAlert(
                        "Bluetooth is off",
                        "Please turn on Bluetooth from Settings.",
                        "Go to Settings",
                        "Cancel"
                    ))
                    {
                        // Open Bluetooth settings
                        await OpenBluetoothSettings();
                    }
                    return false;

                case BluetoothState.Unavailable:
                    Console.WriteLine("Bluetooth is unavailable");
                    await _dialogService.DisplayAlert(
                        "Bluetooth is unavailable",
                        "Please check if your device supports Bluetooth.",
                        "OK"
                    );
                    return false;

                default:
                    Console.WriteLine("Bluetooth state is unknown or transitioning");
                    await _dialogService.DisplayAlert(
                        "Bluetooth state is unknown or transitioning",
                        "Please check the Bluetooth state and try again later.",
                        "OK"
                    );
                    return false;
            }


            // For Android 12 and later
            if (DeviceInfo.Version.Major >= 12)
            {
                // Check BLUETOOTH_SCAN and BLUETOOTH_CONNECT permissions
                var scanStatus = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
                if (scanStatus != PermissionStatus.Granted)
                {
                    scanStatus = await Permissions.RequestAsync<Permissions.Bluetooth>();
                    if (scanStatus != PermissionStatus.Granted)
                    {
                        if (await _dialogService.DisplayAlert(
                            "Bluetooth permission required",
                            "Permission is required to scan for Bluetooth devices. Please allow it from Settings.",
                            "Go to Settings",
                            "Cancel"
                        ))
                        {
                            AppInfo.ShowSettingsUI();
                        }
                        return false;
                    }
                }
            }

            // Check location permission (required for Android 6.0 and later)
            var locationStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (locationStatus != PermissionStatus.Granted)
            {
                locationStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (locationStatus != PermissionStatus.Granted)
                {
                    if (await _dialogService.DisplayAlert(
                        "Location permission required",
                        "Location permission is required to scan for Bluetooth devices. Please allow it from Settings.",
                        "Go to Settings",
                        "Cancel"
                    ))
                    {
                        AppInfo.ShowSettingsUI();
                    }
                    return false;
                }
            }

            bool locationServiceEnabled = IsLocationServiceEnabled();
            if (!locationServiceEnabled)
            {
                if (await _dialogService.DisplayAlert(
                    "Location service is off",
                    "Location service is required to scan for Bluetooth devices. Please enable it from Settings.",
                    "Go to Settings",
                    "Cancel"
                ))
                {
                    await OpenLocationSettings();
                }

                return false;
            }
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlert("Error", $"An error occurred while checking Bluetooth permission: {ex.Message}", "OK");
            return false;
        }

        return true;
    }



    public static async Task OpenBluetoothSettings()
    {
        // Use Android-specific intent to open Bluetooth settings
        var intent = new global::Android.Content.Intent(global::Android.Provider.Settings.ActionBluetoothSettings);

        // Add FLAG_ACTIVITY_NEW_TASK flag
        intent.AddFlags(global::Android.Content.ActivityFlags.NewTask);

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            global::Android.App.Application.Context.StartActivity(intent);
        });
    }

    public static async Task OpenLocationSettings()
    {
        // Use Android-specific intent to open location settings
        var intent = new global::Android.Content.Intent(global::Android.Provider.Settings.ActionLocationSourceSettings);

        // Add FLAG_ACTIVITY_NEW_TASK flag
        intent.AddFlags(global::Android.Content.ActivityFlags.NewTask);

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            global::Android.App.Application.Context.StartActivity(intent);
        });
    }

    public static bool IsLocationServiceEnabled()
    {
        var locationManager = (LocationManager?)Platform.AppContext.GetSystemService(Context.LocationService);
        if (locationManager == null)
            return false;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.P) // API 28 (Android 9.0) and later
        {
            return locationManager.IsLocationEnabled;
        }
        else
        {
            // Fallback for API levels below 28
            try
            {
                var gpsEnabled = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
                var networkEnabled = locationManager.IsProviderEnabled(LocationManager.NetworkProvider);
                return gpsEnabled || networkEnabled;
            }
            catch
            {
                return false;
            }
        }
    }
}
