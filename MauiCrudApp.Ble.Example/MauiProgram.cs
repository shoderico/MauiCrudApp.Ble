using MauiCrudApp.Common.Interfaces;
using MauiCrudApp.Common.Navigation;
using MauiCrudApp.Common.Services;
using Microsoft.Extensions.Logging;

namespace MauiCrudApp.Ble.Example
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Common Services
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<INavigationParameterStore, NavigationParameterStore>();
            builder.Services.AddSingleton<IDialogService, DialogService>();

            // BLE.Services
            builder.Services.AddSingleton<MauiCrudApp.Ble.Interfaces.IBlePlatformService, MauiCrudApp.Ble.Platforms.BlePlatformService>();
            builder.Services.AddSingleton<MauiCrudApp.Ble.Interfaces.IBleDeviceManager, MauiCrudApp.Ble.Services.BleDeviceManager>();
            builder.Services.AddSingleton<MauiCrudApp.Ble.Interfaces.IBleAdapterService, MauiCrudApp.Ble.Services.BleAdapterService>();
            builder.Services.AddSingleton<MauiCrudApp.Ble.Interfaces.IBleCharacteristic, MauiCrudApp.Ble.Models.BleCharacteristic>();
            




            // Feature: DeviceConnect
            builder.Services.AddTransient<Logic.Features.Device.ViewModels.DeviceConnectViewModel<MauiCrudApp.Ble.Views.Features.Device.Views.DeviceScanPage>>();
            builder.Services.AddTransient<Views.Features.Device.Views.DeviceConnectPage>();
            // Feature: DeviceScan
            builder.Services.AddTransient<Logic.Features.Device.ViewModels.DeviceScanViewModel>();
            builder.Services.AddTransient<Views.Features.Device.Views.DeviceScanPage>();

            
            // Feature.Characteristic
            builder.Services.AddSingleton<Logic.Features.Characteristic.ViewModels.CharacteristicStateStore>();
            builder.Services.AddTransient<Logic.Features.Characteristic.ViewModels.CharacteristicControlViewModel>();
            builder.Services.AddTransient<Logic.Features.Characteristic.ViewModels.CharacteristicViewModel>();
            builder.Services.AddTransient<Views.Features.Characteristic.Views.CharacteristicControlPage>();



            // Register AppShell and App
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<App>();

            return builder.Build();
        }
    }
}
