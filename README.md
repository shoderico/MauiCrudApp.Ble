# MauiCrudApp.Ble

**MauiCrudApp.Ble** is a powerful and flexible .NET MAUI library designed to simplify Bluetooth Low Energy (BLE) development across Android, iOS, Windows, and more. Built on top of `MauiCrudApp.Common` and leveraging the `Plugin.BLE` library, it provides a clean, reusable abstraction for scanning, connecting, and interacting with BLE devices. Whether you're reading, writing, or subscribing to characteristics, this library streamlines the process so you can focus on building your app.

The accompanying **MauiCrudApp.Ble.Example** project demonstrates the library's capabilities, guiding you from device discovery to characteristic operations. With the **MauiCrudApp.Ble.ProjectTemplate**, you can kickstart your own BLE projects with ease.

---

## Features

- **Cross-Platform Support**: Works seamlessly on Android, iOS, Windows, and (optionally) Tizen and MacCatalyst.
- **Simplified BLE Operations**: Intuitive APIs for scanning, connecting, and managing BLE devices.
- **Characteristic Management**: Easy read, write, and notification handling for BLE characteristics.
- **MVVM-Friendly**: Integrates with `CommunityToolkit.Mvvm` for clean, reactive UI updates.
- **Permission Handling**: Platform-specific Bluetooth and location permission checks (e.g., Android's location requirements).
- **Error Handling**: Robust error management with clear feedback for connection and operation failures.
- **Example Project**: A fully functional sample app showcasing device scanning, connection, and characteristic interactions.
- **Project Template**: A ready-to-use Visual Studio template to bootstrap your BLE projects.

---

## Project Structure

Below is the file and folder structure for `MauiCrudApp.Ble` and `MauiCrudApp.Ble.Example`, with a brief description of each file's purpose.

### MauiCrudApp.Ble

```
MauiCrudApp.Ble/
├── Interfaces/
│   ├── IBleAdapterService.cs        # Defines BLE adapter operations (scan, connect, etc.)
│   ├── IBleCharacteristic.cs        # Interface for BLE characteristic operations (read, write, notify)
│   ├── IBleDevice.cs                # Interface for BLE device management and state tracking
│   ├── IBleDeviceManager.cs         # Manages BLE device lifecycle (connect, disconnect, reset)
│   ├── IBlePlatformService.cs       # Interface for platform-specific BLE permission checks
│   ├── IBleService.cs               # Interface for BLE service and characteristic management
├── Models/
│   ├── BleCharacteristic.cs         # Implements BLE characteristic with read/write/notify support
│   ├── BleDevice.cs                 # Represents a BLE device with connection state and services
│   ├── BleService.cs                # Manages BLE service and its characteristics
├── Platforms/
│   ├── Android/
│   │   ├── BlePlatformService.cs    # Android-specific BLE permission and location checks
│   ├── iOS/
│   │   ├── BlePlatformService.cs    # iOS-specific BLE state monitoring
│   ├── MacCatalyst/
│   │   ├── BlePlatformService.cs    # Placeholder for MacCatalyst BLE support
│   ├── Tizen/
│   │   ├── BlePlatformService.cs    # Placeholder for Tizen BLE support
│   ├── Windows/
│   │   ├── BlePlatformService.cs    # Placeholder for Windows BLE support
├── Services/
│   ├── BleAdapterService.cs         # Core BLE adapter logic for scanning and connecting
│   ├── BleDeviceManager.cs          # Manages BLE device operations and service discovery
│   ├── BlePlatformServiceBase.cs    # Base class for platform-specific BLE services
├── MauiCrudApp.Ble.csproj           # Project file for the BLE library
```

### MauiCrudApp.Ble.Example

```
MauiCrudApp.Ble.Example/
├── Converters/
│   ├── BoolToNotifyTextConverter.cs  # Converts boolean to notify button text
│   ├── ByteArrayToHexConverter.cs    # Converts byte arrays to hex string
│   ├── ByteArrayToStringConverter.cs # Converts byte arrays to UTF-8 string
├── Features/
│   ├── Characteristic/
│   │   ├── ViewModels/
│   │   │   ├── CharacteristicControlParameter.cs # Navigation parameter for characteristic control
│   │   │   ├── CharacteristicControlViewModel.cs # Manages characteristic UI and operations
│   │   │   ├── CharacteristicStateStore.cs       # Stores and updates characteristic view models
│   │   │   ├── CharacteristicViewModel.cs        # Handles characteristic read/write/notify actions
│   │   ├── Views/
│   │   │   ├── CharacteristicControlPage.xaml    # UI for characteristic interactions
│   │   │   ├── CharacteristicControlPage.xaml.cs # Code-behind for characteristic control page
│   ├── Device/
│   │   ├── ViewModels/
│   │   │   ├── DeviceConnectParameter.cs         # Navigation parameter for device connect
│   │   │   ├── DeviceConnectViewModel.cs         # Manages device connection UI and logic
│   │   │   ├── DeviceScanParameter.cs            # Navigation parameter for device scan
│   │   │   ├── DeviceScanViewModel.cs            # Handles device scanning and selection
│   │   ├── Views/
│   │   │   ├── DeviceConnectPage.xaml            # UI for device connection and services
│   │   │   ├── DeviceConnectPage.xaml.cs         # Code-behind for device connect page
│   │   │   ├── DeviceScanPage.xaml               # UI for scanning BLE devices
│   │   │   ├── DeviceScanPage.xaml.cs            # Code-behind for device scan page
│
├── App.xaml                                     # Application XAML with resources
├── App.xaml.cs                                  # Application setup with shell injection
├── AppShell.xaml                                # Shell navigation with flyout menu
├── AppShell.xaml.cs                             # Code-behind for app shell
├── MainPage.xaml                                # Default home page UI
├── MainPage.xaml.cs                             # Code-behind for home page
├── MauiProgram.cs                               # MAUI app bootstrap and DI setup
```

---

## Getting Started

### Prerequisites

- **.NET 8.0 SDK**
- Visual Studio 2022 (or later) with .NET MAUI workload
- A BLE-capable device for testing
- For Android: Ensure location services and Bluetooth permissions are enabled
- For iOS: Ensure Bluetooth permissions are configured in `Info.plist`

### Installation

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/shoderico/MauiCrudApp.Ble.git
   ```

2. **Add the Library to Your Project**:
   - Reference the `MauiCrudApp.Ble` project directly, or package it as a NuGet package (coming soon!).
   - Ensure `MauiCrudApp.Common` and `Plugin.BLE` are included as dependencies.

3. **Install Dependencies**:
   Update your `.csproj` to include:
   ```xml
   <ItemGroup>
       <PackageReference Include="CommunityToolkit.Mvvm" Version="8.4.0" />
       <PackageReference Include="Plugin.BLE" Version="3.1.0" />
       <PackageReference Include="Microsoft.Maui.Controls" Version="$(MauiVersion)" />
       <PackageReference Include="Microsoft.Maui.Controls.Compatibility" Version="$(MauiVersion)" />
   </ItemGroup>
   ```

4. **Configure Platform Permissions**:
   - **Android**: Update `AndroidManifest.xml` with BLE and location permissions (see `Platforms/Android/AndroidManifest.xml` in the example).
   - **iOS**: Add Bluetooth permissions to `Info.plist` (e.g., `NSBluetoothAlwaysUsageDescription`).

### Using the Project Template

The `MauiCrudApp.Ble.ProjectTemplate` provides a pre-configured Visual Studio project template to bootstrap your BLE apps.

1. Locate the template zip file at `template/ProjectTemplate/binaries/MauiCrudApp.Ble.ProjectTemplate.zip`.
2. Copy the zip file to the Visual Studio project templates folder:
   - Windows: `C:\Users\<YourUsername>\Documents\Visual Studio 2022\Templates\ProjectTemplates`
   - Mac: `~/Library/Visual Studio/Templates/ProjectTemplates`
3. Open Visual Studio, select "Create a new project," and search for `MauiCrudApp.Ble.ProjectTemplate`.
4. Configure the project details (e.g., name, namespace) and create the project.
5. Build and run to start developing your BLE app!

---

## Example Usage

The `MauiCrudApp.Ble.Example` project demonstrates a complete BLE workflow. Here's a quick overview:

### 1. Scanning for Devices
Navigate to the **Device Scan** page to discover nearby BLE devices. The `DeviceScanViewModel` uses `IBleAdapterService` to handle scanning:

```csharp
await _bleAdapterService.StartScanningAsync();
```

The UI displays discovered devices in a `CollectionView`, allowing selection.

### 2. Connecting to a Device
On the **Device Connect** page, select a device and connect using `IBleDeviceManager`:

```csharp
await _bleDeviceManager.ConnectAsync();
```

The page displays the connection state and any errors.

### 3. Interacting with Characteristics
The **Characteristic Control** page lists all services and characteristics for the connected device. You can:
- **Read**: Fetch characteristic values with `ReadAsync`.
- **Write**: Send data with `WriteAsync` (e.g., UTF-8 encoded text).
- **Notify**: Toggle notifications with `StartNotificationsAsync`/`StopNotificationsAsync`.

Example from `CharacteristicViewModel`:

```csharp
[RelayCommand]
private async Task Write(IBleCharacteristic characteristic)
{
    var bytes = System.Text.Encoding.UTF8.GetBytes(WriteValue);
    await characteristic.WriteAsync(bytes);
    WriteValue = string.Empty;
}
```

### Screenshots
*(Add screenshots of the example app here to showcase the UI)*

---

## Contributing

We welcome contributions! To get started:
1. Fork the repository.
2. Create a feature branch (`git checkout -b feature/YourFeature`).
3. Commit your changes (`git commit -m "Add YourFeature"`).
4. Push to the branch (`git push origin feature/YourFeature`).
5. Open a Pull Request.

Please include tests and update documentation as needed.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## Acknowledgments

- Built with ❤️ using .NET MAUI and `Plugin.BLE`.
- Thanks to the `CommunityToolkit.Mvvm` team for awesome MVVM utilities.
- Inspired by the need for simple, cross-platform BLE development.

---

**Ready to build your BLE app?** Clone the repo, try the example, and start connecting with `MauiCrudApp.Ble` today!