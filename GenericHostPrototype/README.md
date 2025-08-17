# GenericHostPrototype

A .NET 10 console application demonstrating the power and flexibility of the .NET Generic Host. This project is a reference for building robust, production-ready console apps using modern .NET hosting patterns.

## Features

- **Dependency Injection**: Register and resolve services using the built-in DI container.
- **Logging**: Configurable logging with environment-specific log levels and providers.
- **Configuration Providers**: Supports JSON files (in `Config/`), environment variables, user secrets, and command-line args.
- **App Lifecycle**: Demonstrates full lifecycle hooks with `IHostedService` and `IHostedLifecycleService`.
- **Health Checks**: Custom health check for system memory and uptime, configurable via options.
- **Background Task Queue**: Channel-based background queue for processing work items asynchronously.
- **Environment Handling**: Uses `DOTNET_ENVIRONMENT` to switch between Development, Staging, and Production, with environment-specific config and behavior.
- **.NET 10 Preview**: Targets .NET 10 (preview) to demo the latest hosting APIs.

## Project Structure

- `Program.cs` – Host setup, configuration, DI, logging, health checks, and environment handling.
- `Config/` – Contains `appsettings.json`, `appsettings.Development.json`, `appsettings.Production.json` (environment-specific configuration).
- `TaskQueue/` – Channel-based background task queue (`BackgroundQueue.cs`, `IBackgroundTaskQueue.cs`) and worker (`QueuedHostedService.cs`).
- `Lifecycle/` – `HostedLifecycleService.cs` demonstrates all lifecycle events of a hosted service.
- `Health/` – `SystemHealthCheck.cs` and `SystemHealthOptions.cs` for custom health checks and options.
- `Properties/launchSettings.json` – Sets `DOTNET_ENVIRONMENT` for local development.
- `GenericHostPrototype.Tests` – xUnit test project for hosted services, health checks, and background queue.

## How It Works

1. **Host Creation**: Uses `Host.CreateApplicationBuilder` to set up the host with configuration, logging, and DI.
2. **Configuration**: Loads settings from JSON (in `Config/`), environment variables, user secrets (in Development), and command-line args. Environment-specific files override base config.
3. **Logging**: Console and debug logging in Development, console-only in Production. Log levels and filters are environment-aware.
4. **Dependency Injection**: Registers services, background queue, and hosted services.
5. **Health Checks**: Adds a custom health check for system memory and uptime, with thresholds set via config.
6. **Background Queue**: Demonstrates queuing and processing background work items (in Development by default).
7. **Lifecycle Events**: HostedLifecycleService logs all lifecycle events for observability.
8. **Environment Handling**: Reads `DOTNET_ENVIRONMENT` to determine environment. Use `Development` for local dev, `Production` for deployment.

## Running the App

1. **Build the solution:**
   ```sh
   dotnet build
   ```
2. **Run the app (default is Production):**
   ```sh
   dotnet run --project GenericHostPrototype/GenericHostPrototype.csproj
   ```
3. **Run in Development mode:**
   ```sh
   set DOTNET_ENVIRONMENT=Development   # Windows
   export DOTNET_ENVIRONMENT=Development # Linux/macOS
   dotnet run --project GenericHostPrototype/GenericHostPrototype.csproj
   ```
4. **Run the tests:**
   ```sh
   dotnet test
   ```

## Switching Environments
- The app uses the `DOTNET_ENVIRONMENT` variable to select the environment.
- Config files like `Config/appsettings.Development.json` and `Config/appsettings.Production.json` override base settings.
- Logging, health checks, and queue size are all environment-aware.

## Extending the Prototype
- Add more hosted/background services.
- Register additional health checks.
- Use the options pattern for more configuration.
- Integrate metrics, OpenTelemetry, or other observability tools.
- Add custom configuration providers.

## Why Use the Generic Host?
- Unified hosting model for console, worker, and web apps.
- Built-in dependency injection and configuration.
- Robust logging and health checks.
- Clean separation of concerns and testability.
- Production-ready patterns out of the box.

## Requirements
- .NET 10 SDK (preview)
