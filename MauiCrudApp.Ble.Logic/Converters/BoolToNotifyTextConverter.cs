using System.Globalization;

namespace MauiCrudApp.Ble.Logic.Converters;

public class BoolToNotifyTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (bool)(value ?? false) ? "Unset Notify" : "Set Notify";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
