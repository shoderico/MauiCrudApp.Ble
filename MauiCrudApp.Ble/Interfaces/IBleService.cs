using Plugin.BLE.Abstractions.Contracts;

namespace MauiCrudApp.Ble.Interfaces;

public interface IBleService
{
    Guid Id { get; }
    string Name { get; }
    IService NativeService { get; }
    IReadOnlyList<IBleCharacteristic> Characteristics { get; }

    void AddCharacteristic(IBleCharacteristic characteristic);
}
