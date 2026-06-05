# NS-LogKit

It is a small, channel-based logging helper for Unity packages and projects. It provides a zero-overhead API (when compile symbols are not defined) to emit logs to the Unity Console with simple channel and level filtering.

What this library provides:

- PackageLogger: a channel-based logger that prefixes messages with the channel name and forwards to the Unity Console via UnityEngine.Debug.
- Per-channel enable/disable and minimum level control via `LogChannel` and `LogChannelRegistry`.
- `LoggingSettings` ScriptableObject (Resources/LoggingSettings) to persist channel configurations that are applied automatically on runtime startup.
- Editor helper hooks (where available) to sync discovered channels into the settings asset.

## Installation

Add the package to your `Packages/manifest.json` to install it from the git repository:

```json
{
  "dependencies": {
    "com.ns.log-kit": "https://github.com/nicolas-stephan/NS-LogKit.git"
  }
}
```

Or point to a specific tag:

```json
"com.ns.log-kit": "https://github.com/nicolas-stephan/NS-LogKit.git#v1.0.0"
```

**Requirements:** Unity 6000+ (or the project-targeted Unity version). Check the documentation for exact compatibility.

Note about compile-time toggles

The logging API is conditionally compiled. To include logging calls in your build define the scripting symbol `ENABLE_LOGS`. To also enable verbose calls, define `ENABLE_VERBOSE_LOG`.

## Quick start

Simple usage example in a Unity script:

```csharp
using NS.LogKit;

public class Example : UnityEngine.MonoBehaviour
{
    private static readonly PackageLogger Log = PackageLogger.For("MyPackage.Physics");

    void Start()
    {
        Log.Info("Started Example component");
        try
        {
            // ... your code ...
        }
        catch (System.Exception ex)
        {
            Log.Error(ex, "An error occurred");
        }
    }
}
```

To configure sinks (log destinations) and global log level, consult the `docs/` folder or the provided editor UI.
To configure channel enablement and minimum level, create a `LoggingSettings` asset via `Assets -> Create -> NS -> Logging Settings` (it is a Resources asset named `LoggingSettings`) and add channel entries. The asset is loaded automatically at runtime (see `LoggingSettings.LoadAndApply`).

Examples for runtime control

You can programmatically control channels with `LogChannelRegistry`:

- `LogChannelRegistry.GetOrCreate("MyPackage.Physics")` — returns a `LogChannel` you can enable/disable or set the `MinLevel` on.
- `LogChannelRegistry.SetGroupEnabled("MyPackage", false)` — disables all channels whose name starts with the prefix.
- `LogChannelRegistry.SetAllEnabled(false)` — master switch to disable package logging at runtime.

## Documentation

Full documentation (guides, API reference, tutorials):

https://nicolas-stephan.github.io/NS-LogKit/ or locally in the `docs/` folder.

## Contributing

Contributions are welcome. Open an issue to discuss major changes and submit pull requests with tests and a clear description of changes.

## License

See the `LICENSE` file for details.