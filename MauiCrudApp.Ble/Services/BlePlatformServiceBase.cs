using MauiCrudApp.Ble.Interfaces;

namespace MauiCrudApp.Ble.Services;

public class BlePlatformServiceBase : IBlePlatformService
{
    public virtual Task<bool> CheckBluetoothPermissionAsync()
    {
        return Task.FromResult(false);
    }
}
