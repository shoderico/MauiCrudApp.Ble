using MauiCrudApp.Ble.Logic.Features.Characteristic.ViewModels;
using MauiCrudApp.Common.Views;

namespace MauiCrudApp.Ble.Views.Features.Characteristic.Views;

public partial class CharacteristicControlPage : PageBase
{
    public CharacteristicControlPage(CharacteristicControlViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}