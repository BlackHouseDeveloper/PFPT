# Runtime Targets

- Web (net8.0): DB-stateless, API-only. No EF/SQLite. Any caching is ephemeral (memory/IndexedDB/local storage).
- Devices (net8.0-android/ios/maccatalyst): EF Core + SQLite for local storage and offline. Sync to API.

## Sync Model

- Source of truth: API.
- Concurrency: ETags on GET; `If-Match` on PUT. Conflicts return `412 Precondition Failed`.
- Delta syncs: `updatedSince` parameter (ISO 8601) for changed records.