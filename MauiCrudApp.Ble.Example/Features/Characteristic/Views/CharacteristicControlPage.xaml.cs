using MauiCrudApp.Ble.Example.Features.Characteristic.ViewModels;
using MauiCrudApp.Common.Views;

namespace MauiCrudApp.Ble.Example.Features.Characteristic.Views;

public partial class CharacteristicControlPage : PageBase
{
    public CharacteristicControlPage(CharacteristicControlViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}