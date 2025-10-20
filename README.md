# SoundHz

SoundHz is a friendly cross-platform soundboard that lets you trigger your favorite audio clips with a tap. Whether you're queuing up reaction sounds for a live stream or organizing quick-reference clips for an event, SoundHz keeps everything one touch away on Android, iOS, macOS (Mac Catalyst), and Windows.

## What you can do

* Browse all of your previously created sound boards from a single home screen.
* Add new boards or expand existing ones with custom sounds using the built-in file picker.
* Tap any tile to play one or more clips simultaneously using the Community Toolkit `MediaElement` player.
* Long-press a tile to edit or delete a sound's name, description, or backing audio file.
* Automatically persist board changes so your setup is ready the next time you open the app.

## Behind the scenes (for developers)

### Prerequisites

* .NET SDK 10 preview with C# language version 14 enabled.
* .NET MAUI workload installed for Android, iOS, Mac Catalyst, and Windows targets (`dotnet workload install maui` and the appropriate platform workloads).
* Platform-specific tooling:
  * Android SDK / emulator (Visual Studio or Android Studio).
  * Xcode with iOS simulator for Apple platforms.
  * Visual Studio 2022 17.8+ on Windows for Windows builds.

### Building and running

```bash
# Restore packages
dotnet restore

# Build all targets
dotnet build SoundHz.sln

# Run a platform target (example: Windows)
dotnet build -t:Run -f net10.0-windows10.0.19041.0 src/SoundHz.csproj
```

> **Tip:** Use `maui check` to verify your environment and `dotnet workload install` to add any missing platform workloads before building.

### Solution structure

```text
SoundHz.sln
└── src/
    ├── App.xaml / App.xaml.cs        # Bootstraps the MAUI application shell
    ├── MauiProgram.cs               # Configures dependency injection and services
    ├── appsettings.json             # Stores persisted sound board definitions
    ├── Models/                      # Strongly typed configuration entities
    ├── Services/                    # Persistence, playback, and navigation services
    ├── CommandHandling/             # Command handlers for saving boards
    ├── ViewModels/                  # MVVM view models for each page
    ├── Views/                       # XAML pages for listing and managing sounds
    └── Resources/                   # Localization resources and assets
```

### Major components

* **Models (`src/Models/`)** – Define sound boards and sound tiles using validated configuration classes.
* **Services (`src/Services/`)** – Handle file access, JSON serialization, repository operations, dependency injection factories, and audio playback lifecycles.
* **ViewModels (`src/ViewModels/`)** – Provide MVVM bindings for the UI, exposing observable collections and commands for navigation, playback, and editing flows.
* **Views (`src/Views/`)** – XAML pages that present the board list, sound grid, and add/edit dialogs.
* **Resources (`src/Resources/`)** – Localization resources for logging and error handling strings.

With these pieces, SoundHz keeps the UI responsive, the audio playback reliable, and your sound boards synced across sessions.
