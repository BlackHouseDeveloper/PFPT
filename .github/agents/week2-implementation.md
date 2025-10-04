# Week 2 Roadmap Implementation Agent

## Purpose
This agent implements Week 2 features of the Physically Fit PT 2-Month Roadmap, focusing on authentication stubs, AI note generation prototypes, complete navigation, and developer diagnostics.

## Scope
The agent should handle:
- Creating authentication service stubs with hard-coded demo credentials
- Implementing AI note generation prototypes with mock output
- Ensuring all roadmap pages are accessible via navigation
- Maintaining developer diagnostics panels on all pages
- Setting up dependency injection for shared services across Web and MAUI

## Key Principles
1. **Minimal Changes**: Make surgical, focused changes that don't break existing functionality
2. **Code Reuse**: Leverage shared Razor Class Library (PhysicallyFitPT.Shared) for UI components
3. **Cross-Platform**: Ensure services work on both Blazor WebAssembly (Web) and MAUI platforms
4. **Week 2 Focus**: Implement stubs and prototypes, not production-ready features
5. **Clean Architecture**: Keep interfaces in Shared, implementations in Infrastructure/AI projects

## Architecture Guidelines

### Project Structure
```
PhysicallyFitPT/
├── src/
│   ├── PhysicallyFitPT.Shared/        # Shared Blazor components, interfaces, DTOs
│   ├── PhysicallyFitPT.Infrastructure/ # Service implementations, data access
│   ├── PhysicallyFitPT.AI/            # AI service implementations
│   ├── PhysicallyFitPT.Web/           # Blazor WebAssembly host
│   └── PhysicallyFitPT.Maui/          # MAUI Blazor host
```

### Dependency Flow (Must Avoid Circular References)
- `Shared` contains interfaces and DTOs (no dependencies on Infrastructure/AI)
- `Infrastructure` and `AI` implement interfaces from `Shared`
- `Web` and `Maui` reference all projects and wire up DI

### Service Registration Pattern
Services should be registered in both Web and MAUI:

**Web (Program.cs):**
```csharp
builder.Services.AddSingleton<IUserService, PhysicallyFitPT.Infrastructure.Services.UserService>();
builder.Services.AddSingleton<IAiNoteService, PhysicallyFitPT.AI.AiNoteService>();
```

**MAUI (MauiProgram.cs):**
```csharp
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IAiNoteService, PhysicallyFitPT.AI.AiNoteService>();
```

## Week 2 Implementation Details

### Authentication Service
- **Interface**: `IUserService` in `PhysicallyFitPT.Shared`
- **Implementation**: `UserService` in `PhysicallyFitPT.Infrastructure.Services`
- **Demo Credentials**:
  - Username: `clinician@demo.com`
  - Password: `demo123`
  - Display Name: `Dr. Demo Clinician`
- **Methods**: `LoginAsync`, `LogoutAsync`, `GetAccessTokenAsync`, `RefreshTokenAsync`
- **State**: In-memory ClaimsPrincipal for current user

### AI Note Service
- **Interface**: `IAiNoteService` in `PhysicallyFitPT.Shared`
- **Result DTO**: `AiNoteSummaryResult` in `PhysicallyFitPT.Shared`
- **Implementation**: `AiNoteService` in `PhysicallyFitPT.AI`
- **Behavior**: Simulates 1.5s processing delay, generates mock SOAP note
- **Output Format**: Includes Subjective, Objective, Assessment, Plan sections
- **Prototype Notice**: Clearly labels output as prototype/mock

### Navigation Structure
All pages should be in the `ResponsiveNavMenu.razor`:
- 🏠 Dashboard (/)
- 🔐 Login (/login)
- 👥 Patients (/patients)
- 📋 Patient Intake (/intake)
- 📅 Appointments (/appointments)
- 📝 Eval Note (/notes/eval)
- 📄 Daily Note (/notes/daily)
- 📈 Progress Note (/notes/progress)
- 📋 Discharge Note (/notes/discharge)
- 🤖 AI Summary (/ai/summary)
- ⚙️ Settings (/admin/settings)
- 👤 User Management (/admin/users)
- 📨 Auto Messaging (/admin/automessaging)
- 📤 Export Data (/admin/export)
- 🎨 UI Kit (/ui-kit)

### Page Requirements
Each page should:
1. Include `<DebugStatBar />` component at the top
2. Use `ResponsiveMainLayout` layout
3. Have clear placeholder content or working functionality
4. Display "Week 2" notices for stub/prototype features

### Developer Diagnostics
The `DebugStatBar` component should:
- Only show in DEBUG builds or when developer mode is enabled
- Display: Patient count, last update timestamp, appointment count, API health
- Auto-refresh every 10 seconds with jitter to avoid thundering herd
- Be visible on all pages in development mode

## Testing Guidelines

### Build Testing
```bash
# Test Web build
dotnet build src/PhysicallyFitPT.Web/PhysicallyFitPT.Web.csproj

# Test Infrastructure build
dotnet build src/PhysicallyFitPT.Infrastructure/PhysicallyFitPT.Infrastructure.csproj
```

### Runtime Testing
```bash
# Start API (required for data service)
cd src/PhysicallyFitPT.Api && dotnet run

# Start Web (in new terminal)
cd src/PhysicallyFitPT.Web && dotnet run
```

Test scenarios:
1. **Authentication**: Login with demo credentials, verify navigation works
2. **AI Summary**: Enter clinical notes, click generate, verify mock SOAP note appears
3. **Navigation**: Click all menu items, verify pages load without errors
4. **Diagnostics**: Verify debug panel shows on all pages in dev mode

## Common Issues & Solutions

### Circular Dependency Error
**Problem**: `error MSB4006: There is a circular dependency in the target dependency graph`

**Solution**: Never add Infrastructure or AI project references to Shared. Interfaces go in Shared, implementations go in Infrastructure/AI.

### Service Not Found Error
**Problem**: `InvalidOperationException: Unable to resolve service for type 'IUserService'`

**Solution**: Ensure service is registered in both Web Program.cs and MAUI MauiProgram.cs

### Razor Component Not Found
**Problem**: `Found markup element with unexpected name 'EditForm'`

**Solution**: Use standard HTML elements (`<form>`, `<input>`) or add `@using Microsoft.AspNetCore.Components.Forms` to `_Imports.razor`

### Navigation Not Working in Blazor WASM
**Problem**: `Navigation.NavigateTo("/", forceLoad: true)` doesn't work

**Solution**: Use `Navigation.NavigateTo("/")` without `forceLoad` in Blazor WebAssembly

## Future Enhancements (Beyond Week 2)

Week 3-4 (per roadmap):
- Replace hard-coded credentials with actual authentication
- Add patient intake form with data binding and validation
- Enhance AI service with real OpenAI/Azure OpenAI integration
- Add persistent storage (SQLite/EF Core)

Week 5-6 (per roadmap):
- Complete progress notes functionality
- Implement export service with CSV/JSON generation
- Add admin settings and user management

## Code Style

- Use file-scoped namespaces
- Include XML documentation comments on public members
- Follow existing code patterns in the repository
- Use StyleCop and Roslynator for code quality
- Keep methods focused and single-purpose

## Success Criteria

Week 2 implementation is complete when:
- ✅ Login page accepts demo credentials and tracks auth state
- ✅ AI Summary page generates mock SOAP notes from clinical input
- ✅ All 15 roadmap pages are accessible via navigation
- ✅ Developer diagnostics panel shows on all pages in dev mode
- ✅ Services are registered in both Web and MAUI DI containers
- ✅ Web project builds and runs successfully
- ✅ No circular dependency errors in project references
