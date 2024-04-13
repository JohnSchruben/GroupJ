# SafeSkate

SafeSkate is a mobile application designed for skaters to crowdsource and report sidewalk and road conditions. It is developed using C# and Xamarin, including a server backend. This project is developed by Group J as part of CS 3203 - Software Engineering at The University of Oklahoma.

---

## Features

- Crowdsourcing sidewalk and road conditions
- Reporting hazards such as potholes, large cracks, and bumps
- Interactive map interface

## Getting Started

These instructions will help you set up and run SafeSkate on your development environment.

### Prerequisites

- Visual Studio
- Rider (for macOS)
- Xcode (for macOS)
- .NET Core
- Xamarin

---

## Simulating iOS and Android

On Windows, you can run the `SafeSkate.Mobile.Droid` project in an Android emulator. Likewise, on macOS, you can run the `SafeSkate.Mobile.iOS` project in an iOS emulator.

---

## Building IPA for iOS (macOS)

To install Xamarin on macOS, first download and run the Visual Studio for Mac Installer. From here, install the following from the installer if not already installed.

- .NET
- Android
- iOS
- macOS Cocoa (Optional)

1. Clone the repository.
```bash
git clone https://github.com/JohnSchruben/GroupJ.git
```
2. Open the solution file `/Application/Application.sln` in Rider for macOS.
3. Select `SafeSkate.Mobile.iOS` as the target project.
4. Set the Configuration to `Debug` or `Release` and the Platform to `iPhone`.
		Note: If you only want to debug in a simulator on macOS, set the Platform to `iPhoneSimulator` and the Mac Agent to any Simulated device. You can run this directly without a physical device.
5. Open the Xcode project in `RiderApp/RiderApp.xcodeproj`.
6. Connect your device via USB to your Mac.
7. Navigate to Xcode > Settings > Accounts.
8. Add your Apple ID.
9. Navigate to Project Settings > Singing & Capabilities > Team.
10. Select your Apple ID as the Team for this project.
11. Run the empty Xcode project with your device as the run target.
12. It should automatically download RiderApp onto your device and run.
13. Go back to the Rider project and change the Mac Agent to your device.
14. Run the `SafeSkate.Mobile.iOS` project with your device as the run target.
15. It should automatically download SafeSkate onto your device and run.

Note: Steps 5 through 12 are to have your device registered to your Apple ID under a manually-created provisioning profile. The Bundle Identifiers between the Xcode project and the Rider solution `SafeSkate.Mobile.iOS/Info.plist` must be identical.

---

## Building APK for Android

1. Clone the repository.
```bash
git clone https://github.com/JohnSchruben/GroupJ.git
```
2. Open the solution file `/Application/Application.sln` in your IDE.
3. Select `SafeSkate.Mobile.Droid` as the target project.
4. Set the Configuration to `Debug` or `Release` and the Platform to `Any CPU`.
		Note: If you only want to debug in a simulator on Windows, simply select your Android Emulator from build targets. You can run this directly without a physical device.
5. Right-click the project and click `Archive for publishing`.
6. Sign and publish your project.

Note: Steps 6 through 7 were written with Rider for both Windows and macOS in mind. There may be differences in publishing an APK in Visual Studio that have not been covered here.

---

## Building the Server
1. Clone the repository.
```bash
git clone https://github.com/JohnSchruben/GroupJ.git
```
2. Open the solution file `/Backend/Backend.sln` in your IDE.
3. Select `SafeSkate.Service` as the target project.
4. Set the Configuration to `Service`.
5. Build and run the project.

---

## Sideloading on iOS

If you are unable to build the project for iOS, for example you do not have access to a device running macOS, then you must grab a prebuilt IPA from the repository releases and sideload manually.

1. Transfer the `.IPA` file to your computer.
2. Install the `.IPA` file on your iOS device using [AltStore](https://faq.altstore.io/).

---

## Sideloading on Android

1. Once you have built the `SafeSkate.Mobile.Droid` project and archived for publishing, find your `.APK` file.
2. Transfer the `.APK` file to your Android device.
3. On your Android device, enable the installation of apps from unknown sources in the settings menu.

---

## Contributing

We welcome contributions to SafeSkate! If you'd like to contribute, please follow these steps:

1. Create a new branch (`git checkout -b feature_name/your-feature_name`).
2. Make your changes.
3. Commit your changes (`git commit -am 'Add new feature'`).
4. Push to the branch (`git push origin feature_name/your-feature_name`).
5. Create a new Pull Request.

---

## License

TODO: Add project license.

---

## Acknowledgments

- [Xamarin](https://dotnet.microsoft.com/apps/xamarin)
- [Visual Studio](https://visualstudio.microsoft.com/downloads/)
- [Rider](https://www.jetbrains.com/rider/download/#section=windows)
- [Xcode](https://developer.apple.com/xcode/)
