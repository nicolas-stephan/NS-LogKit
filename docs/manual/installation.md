# Installation

To add NS-LogKit to your Unity project, use the Package Manager by adding the package to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.ns.log-kit": "https://github.com/nicolas-stephan/NS-LogKit.git"
  }
}
```

Alternatively, pin a specific release tag:

```json
"com.ns.log-kit": "https://github.com/nicolas-stephan/NS-LogKit.git#v1.1.4"
```

Requirements

- Unity 2024.1 or newer (package manifest targets `6000.0`).

Compile-time toggles

NS-LogKit uses conditional compilation to remove logging calls when logging is disabled.

- `ENABLE_LOGS` — enables `PackageLogger` log methods.
- `ENABLE_VERBOSE_LOG` — additionally enables `Verbose(...)` methods when `ENABLE_LOGS` is defined.

Define these symbols in Player Settings → Scripting Define Symbols, or in the assembly definition settings for the package consumer assembly.
