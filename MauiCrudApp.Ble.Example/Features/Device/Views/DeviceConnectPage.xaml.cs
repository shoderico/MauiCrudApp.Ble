using MauiCrudApp.Ble.Example.Features.Device.ViewModels;
using MauiCrudApp.Common.Views;

namespace MauiCrudApp.Ble.Example.Features.Device.Views;

public partial class DeviceConnectPage : PageBase
{
    public DeviceConnectPage(DeviceConnectViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}