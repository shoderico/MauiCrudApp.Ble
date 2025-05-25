using MauiCrudApp.Ble.Logic.Features.Device.ViewModels;
using MauiCrudApp.Common.Views;

namespace MauiCrudApp.Ble.Views.Features.Device.Views;

public partial class DeviceConnectPage : PageBase
{
    public DeviceConnectPage(DeviceConnectViewModel<DeviceScanPage> viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}