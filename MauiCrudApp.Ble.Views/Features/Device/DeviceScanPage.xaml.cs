using MauiCrudApp.Ble.Logic.Features.Device.ViewModels;
using MauiCrudApp.Common.Views;

namespace MauiCrudApp.Ble.Views.Features.Device.Views;

public partial class DeviceScanPage : PageBase
{
    public DeviceScanPage(DeviceScanViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}