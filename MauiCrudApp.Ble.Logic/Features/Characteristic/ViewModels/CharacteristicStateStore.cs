﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

using MauiCrudApp.Common.Interfaces;
using MauiCrudApp.Ble.Interfaces;

namespace MauiCrudApp.Ble.Logic.Features.Characteristic.ViewModels;

public partial class CharacteristicStateStore : ObservableObject
{
    private readonly IBleDeviceManager _bleDeviceManager;
    private readonly IDialogService _dialogService;

    public ObservableCollection<CharacteristicViewModel> Characteristics { get; private set; } = new();

    public CharacteristicStateStore(IBleDeviceManager bleDeviceManager, IDialogService dialogService)
    {
        _bleDeviceManager = bleDeviceManager;
        _dialogService = dialogService;
        _bleDeviceManager.BleDevice.ServicesChanged += OnServicesChanged;
    }

    private async void OnServicesChanged(object? sender, EventArgs e)
    {
        await UpdateCharacteristicsAsync();
    }

    public async Task UpdateCharacteristicsAsync()
    {
        try
        {
            var services = _bleDeviceManager.BleDevice.Services.ToList() // copied list
                         ?? new List<IBleService>();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                try
                {
                    var currentServiceIds = services.Select(s => s.Id).ToHashSet();
                    var toRemove = Characteristics
                        .Where(cvm => !currentServiceIds.Contains(cvm.Characteristic.Id))
                        .ToList();
                    foreach (var cvm in toRemove)
                    {
                        OnRemoveCharacteristicViewModel(cvm);
                    }

                    foreach (var service in services)
                    {
                        foreach (var characteristic in service.Characteristics)
                        {
                            var existingCvm = Characteristics.FirstOrDefault(
                                cvm => cvm.Characteristic.Id == characteristic.Id &&
                                       cvm.Characteristic.Id == service.Id);

                            if (existingCvm == null)
                            {
                                Console.WriteLine($"Creating new CharacteristicViewModel for {characteristic.Id}");
                                var newCvm = CreateCharacteristicViewModel(characteristic, service.Id, _bleDeviceManager, _dialogService);
                                Characteristics.Add(newCvm);
                            }
                            else
                            {
                                Console.WriteLine($"Reusing CharacteristicViewModel for {characteristic.Id}");
                                existingCvm.Characteristic = characteristic;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    _dialogService.DisplayAlert("Error", $"Failed to update characteristics in main thread: {ex.Message}", "OK");
                }
            });
        }
        catch (Exception ex)
        {
            await _dialogService.DisplayAlert("Error", $"Failed to update characteristics: {ex.Message}", "OK");
        }
    }

    protected virtual CharacteristicViewModel CreateCharacteristicViewModel(IBleCharacteristic characteristic, Guid id, IBleDeviceManager bleDeviceManager, IDialogService dialogService)
    {
        return new CharacteristicViewModel(characteristic, id, bleDeviceManager, dialogService);
    }

    protected virtual void OnRemoveCharacteristicViewModel(CharacteristicViewModel cvm)
    {
        Characteristics.Remove(cvm);
    }

    
}
