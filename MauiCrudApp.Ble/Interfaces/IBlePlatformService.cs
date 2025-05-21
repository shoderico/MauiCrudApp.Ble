namespace MauiCrudApp.Ble.Interfaces;

public interface IBlePlatformService
{
    Task<bool> CheckBluetoothPermissionAsync();
}

