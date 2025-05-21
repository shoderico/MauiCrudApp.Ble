using MauiCrudApp.Ble.Example.Features.Device.ViewModels;
using MauiCrudApp.Common.Views;

namespace MauiCrudApp.Ble.Example.Features.Device.Views;

public partial class DeviceScanPage : PageBase
{
    public DeviceScanPage(DeviceScanViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}