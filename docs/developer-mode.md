# Developer Diagnostics Mode

PFPT ships with a developer diagnostics bar that can be surfaced across Blazor WebAssembly and .NET MAUI hosts. To keep production deployments secure by default, diagnostics are hidden unless explicitly enabled.

## Precedence Order

When the diagnostics component (`DebugStatBar`) initializes, it applies the first matching configuration source, in this order:

1. `PFPT_DEVELOPER_MODE` environment variable (`true`/`false`, case-insensitive)
2. `App:DeveloperMode` setting from the active configuration (e.g., `appsettings.json`)
3. Build-default fallback (`true` for debug builds, `false` for release builds)

This allows production operators to temporarily opt in without redeploying.

## Platform Notes

- **.NET MAUI (mobile/desktop)**: Environment variables are read from the process environment. On macOS/Linux export the variable before launch (`PFPT_DEVELOPER_MODE=1 ./run-pfpt.sh`). On Windows PowerShell use `setx PFPT_DEVELOPER_MODE 1` (new shells only) or `$env:PFPT_DEVELOPER_MODE='1'` for a single session.
- **MAUI Android**: Environment variables are not persisted between launches. Use `appsettings.{Environment}.json` to set `"App": { "DeveloperMode": true }` for targeted builds, or push a temporary system property via `adb shell setprop debug.pfpt.developer_mode 1` and hydrate configuration accordingly.
- **MAUI iOS**: Define `PFPT_DEVELOPER_MODE` in the scheme's environment overrides (Debug > Edit Scheme) or supply an `AppSettings.Production.json` with the configuration key when diagnostics are intentionally exposed.
- **Blazor WebAssembly**: Browser sandboxes do not surface OS environment variables, so the override typically resolves to the configuration setting or the build default.
- **Reverse proxy hosting**: If the API sits behind a base path (e.g. `/pfpt`), set `Api:BasePath` so generated routes and diagnostics endpoints remain reachable.

## Quick Commands

```bash
# macOS/Linux shell
PFPT_DEVELOPER_MODE=1 dotnet run --project src/PhysicallyFitPT.Api

# Windows PowerShell
$env:PFPT_DEVELOPER_MODE='1'; dotnet run --project src/PhysicallyFitPT.Api

# Android emulator (temporary)
adb shell setprop debug.pfpt.developer_mode 1
```

## Observability

When diagnostics are enabled via the environment variable on a non-debug build, the component emits a warning log entry so operations teams can audit exposure.

## Suggested Usage

- Leave `App:DeveloperMode` unset (or `false`) in production configuration.
- Use the environment variable only during scheduled troubleshooting windows, and unset it when finished. The component emits a single warning per process when this happens in release builds.
- Pair the diagnostics bar with appropriate feature flags or access controls if you expose the application over the public internet.
- Consider adding a visible non-production watermark whenever developer diagnostics are active so operators can confirm the state without checking logs.
- Ship an explicit configuration entry when necessary. Example:

```json
{
  "App": {
    "DeveloperMode": true
  }
}
```

## Configuration Snippets

```json
{
  "Api": { "BasePath": "/pfpt" },
  "AppStats": { "CacheTtlSeconds": 15 },
  "App": { "DiagnosticsRequiredRole": "Operator" }
}
```

Environment equivalent:

```bash
export PFPT__API__BASEPATH="/pfpt"
export PFPT__APPSTATS__CACHETTLSECONDS=15
export PFPT__APP__DIAGNOSTICSREQUIREDROLE="Operator"
```

## Troubleshooting Diagnostics

- **No banner visible**
  - Confirm you are running a Debug build or have explicitly set `App:DeveloperMode`.
  - Ensure the precedence order (env var → config → default) resolves to `true`.
  - Verify `Api:BasePath` correctly reflects any reverse-proxy prefix.
- **Endpoint returns 401**: Authentication is required in non-development environments before diagnostics state is disclosed.
- **Endpoint returns 403**: The caller is authenticated but does not meet the optional `App:DiagnosticsRequiredRole` requirement.
- **Endpoint returns 404**: This is expected when diagnostics are disabled; it does not signal an outage.
- **WebAssembly override ignored**: Use configuration files/scripts—browser sandboxes do not surface environment variables.
- **Need faster stats refresh?** Override `AppStats:CacheTtlSeconds` (default 15) in `appsettings.{Environment}.json`.
- **Validate runtime state**: Call `GET /api/v1/diagnostics/info`; when diagnostics are enabled the response contains `PFPT-Diagnostics: true`. Use `GET /health/info` for a lightweight status payload.
