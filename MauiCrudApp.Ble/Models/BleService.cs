using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Plugin.BLE.Abstractions.Contracts;

using MauiCrudApp.Ble.Interfaces;

namespace MauiCrudApp.Ble.Models;

public partial class BleService : ObservableObject, IBleService
{
    [ObservableProperty]
    private Guid id = Guid.Empty;

    [ObservableProperty]
    private string name = "";

    public IService NativeService { get; private set; }

    private readonly ObservableCollection<IBleCharacteristic> _characteristics = new();
    public IReadOnlyList<IBleCharacteristic> Characteristics => _characteristics;

    public BleService(Guid id, string name, IService nativeService)
    {
        Id = id;
        Name = name ?? "Unknown Service";
        NativeService = nativeService;
    }

    public void AddCharacteristic(IBleCharacteristic characteristic)
    {
        _characteristics.Add(characteristic);
    }
}
