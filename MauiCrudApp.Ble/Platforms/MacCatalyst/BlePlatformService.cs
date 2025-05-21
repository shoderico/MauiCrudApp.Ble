using MauiCrudApp.Common.Interfaces;
using MauiCrudApp.Ble.Services;

namespace MauiCrudApp.Ble.Platforms;

public class BlePlatformService : BlePlatformServiceBase
{
    private IDialogService _dialogService;

    public BlePlatformService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }
}
