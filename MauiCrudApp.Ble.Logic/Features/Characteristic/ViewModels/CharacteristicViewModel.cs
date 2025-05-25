using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MauiCrudApp.Common.Interfaces;
using MauiCrudApp.Ble.Interfaces;
using Plugin.BLE.Abstractions;

namespace MauiCrudApp.Ble.Logic.Features.Characteristic.ViewModels;

public partial class CharacteristicViewModel : ObservableObject
{
    private readonly IBleDeviceManager _bleDeviceManager;
    private readonly IDialogService _dialogService;
    private readonly Guid _serviceId;

    [ObservableProperty]
    private IBleCharacteristic characteristic;

    [ObservableProperty]
    private string writeValue;

    [ObservableProperty]
    private ObservableCollection<BleValueChangedEventArgs> readValues;

    [ObservableProperty]
    private ObservableCollection<CharacteristicWriteType> writeTypeOptions;

    public CharacteristicViewModel(IBleCharacteristic characteristic, Guid serviceId, IBleDeviceManager bleDeviceManager, IDialogService dialogService)
    {
        Characteristic = characteristic;
        _serviceId = serviceId;
        _bleDeviceManager = bleDeviceManager;
        _dialogService = dialogService;
        WriteValue = string.Empty;
        ReadValues = new ObservableCollection<BleValueChangedEventArgs>();

        WriteTypeOptions = new ObservableCollection<CharacteristicWriteType>
        {
            CharacteristicWriteType.Default,
            CharacteristicWriteType.WithResponse,
            CharacteristicWriteType.WithoutResponse
        };

        // Subscribe to value changes
        characteristic.ValueChanged += OnCharacteristicValueChanged;
    }

    protected virtual void OnCharacteristicValueChanged(object? s, BleValueChangedEventArgs value)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            OnPropertyChanged(nameof(Characteristic));
            ReadValues.Add(value);
        });
    }



    [RelayCommand]
    private async Task Write(IBleCharacteristic characteristic)
    {
        try
        {
            if (string.IsNullOrEmpty(WriteValue))
            {
                await _dialogService.DisplayAlert("Error", "Please enter text to send.", "OK");
                return;
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(WriteValue);
            await characteristic.WriteAsync(bytes);
            WriteValue = string.Empty; // Clear input after sending
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlert("Error", $"Failed to write: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task Read(IBleCharacteristic characteristic)
    {
        try
        {
            var value = await characteristic.ReadAsync();
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlert("Error", $"Failed to read: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task ToggleNotify(IBleCharacteristic characteristic)
    {
        try
        {
            if (characteristic.IsNotifying)
            {
                await characteristic.StopNotificationsAsync();
            }
            else
            {
                await characteristic.StartNotificationsAsync();
            }
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlert("Error", $"Failed to toggle notification: {ex.Message}", "OK");
        }
    }
}
