# Browser vs Mobile Data Storage

This document explains the different data storage approaches used by PFPT across platforms.

## Overview

The PFPT application uses a **platform-specific data storage abstraction** to handle the differences between browser and mobile environments:

- **Mobile/Desktop**: Uses SQLite with Entity Framework Core for persistent storage
- **Browser (WebAssembly)**: Uses in-memory Entity Framework provider for browser compatibility

## Architecture

### IDataStore Interface

All data operations go through the `IDataStore` interface, which provides platform-agnostic CRUD operations for:
- Patients
- Appointments 
- Notes
- Questionnaires
- Reference data (CPT codes, ICD-10 codes)
- Check-in messaging

### Platform-Specific Implementations

#### SqliteDataStore (Mobile/Desktop)
- **Platform**: Android, iOS, Windows, macOS
- **Storage**: SQLite database file
- **Persistence**: Data persists between app sessions
- **Location**: Device local storage (`FileSystem.AppDataDirectory`)
- **Advantages**: Full SQL capabilities, data persistence, offline access

#### BrowserDataStore (WebAssembly)
- **Platform**: Web browsers via WebAssembly
- **Storage**: In-memory Entity Framework provider
- **Persistence**: Data lost on page refresh
- **Location**: Browser memory
- **Advantages**: No native dependencies, browser sandbox compatible
- **Auto-seeding**: Includes basic CPT and ICD-10 reference data

## Platform Detection

Platform-specific data stores are registered at startup:

```csharp
// Mobile (MauiProgram.cs)
builder.Services.AddScoped<IDataStore, SqliteDataStore>();

// Browser (Program.cs)  
builder.Services.AddScoped<IDataStore, BrowserDataStore>();
```

## Fixed Issues

### WebAssembly SQLite Crash (e_sqlite3)

**Problem**: The original WebAssembly build crashed with `e_sqlite3` errors because it tried to use native SQLite libraries that aren't compatible with the browser sandbox.

**Solution**: 
1. Removed native SQLite initialization (`SQLitePCL.Batteries_V2.Init()`) from WebAssembly builds
2. Excluded SQLite native file references from WASM project
3. Created browser-compatible data store using in-memory EF provider
4. Added graceful error handling for storage initialization

### Build Size Optimization

**Improvements**:
- Installed `wasm-tools` workload for better WebAssembly support
- Enabled trimming (`PublishTrimmed=true`) to reduce payload size
- Current published size: ~20MB (down from potentially larger untrimmed builds)

## Diagnostics

Visit `/diagnostics` in the web application to see:
- Active storage backend
- Platform information  
- Data store initialization status
- Storage health test results

## Usage Notes

### For Developers

1. **Use IDataStore**: Always interact with data through the `IDataStore` interface, never directly with EF contexts
2. **Handle Browser Limitations**: Remember that browser storage is temporary - implement data export/import if needed
3. **Test Both Platforms**: Verify functionality works on both mobile and browser platforms

### For Users

1. **Mobile Apps**: Data persists between sessions, works offline
2. **Web App**: Data is temporary, will be lost on page refresh
3. **Data Migration**: No automatic sync between platforms (future enhancement)

## Future Enhancements

Potential improvements:
- IndexedDB implementation for browser persistence
- Data synchronization between platforms
- Progressive Web App (PWA) support with service workers
- Cloud backup/restore functionality

## Testing

The solution includes comprehensive tests:
- `DataStoreTests`: Verifies both implementations work correctly
- Platform-specific initialization tests
- CRUD operation validation
- Reference data seeding verification

Run tests with: `dotnet test PhysicallyFitPT.Tests/PhysicallyFitPT.Tests.csproj`