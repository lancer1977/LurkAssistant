# LurkAssistant

Window/app switch helper for lurk workflows.

## CLI usage

LurkAssistant loads settings from `src/appsettings.json` and supports command-line overrides.

### Supported options

- `--help` / `-h` → prints all available options
- `--streamer <name>` → overrides the lurk target using a single streamer name
- `--delay <seconds>` → overrides `IterationDelay`
- `--window-swap-delay <seconds>` → overrides `WindowSwapDelay`
- `--app-name <name>` → overrides `AppName`
- `--lurks <comma,separated,list>` → overrides `Lurks`
- `--browser <path>` → overrides `BrowserExecutable`

### Examples

```bash
# Run with defaults from appsettings.json
dotnet run --project src/LurkHelper.csproj

# Override some values from CLI
dotnet run --project src/LurkHelper.csproj -- --delay 10 --app-name firefox --browser /usr/bin/firefox
```


## 📖 Documentation
Detailed documentation can be found in the following sections:
- [Feature Index](./docs/features/README.md)
- [Core Capabilities](./docs/features/core-capabilities.md)
