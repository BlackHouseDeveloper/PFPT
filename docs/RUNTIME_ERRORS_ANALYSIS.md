# Runtime Errors Analysis for PFPT Application

## Overview
This document identifies potential runtime errors found in the PhysicallyFitPT application codebase through systematic analysis of all C# source files.

## Critical Runtime Error Categories

### 1. Null Reference Exceptions

#### High Risk
- **PatientService.SearchAsync()** (Line 15)
  ```csharp
  var q = (query ?? string.Empty).Trim().ToLower();
  ```
  **Issue**: While this line handles null, the parameter could still cause issues if the consumer passes unexpected values.

- **Entity Navigation Properties**
  ```csharp
  public Patient Patient { get; set; } = null!;
  ```
  **Issue**: Using `null!` suppresses nullable warnings but doesn't prevent runtime nulls.

#### Medium Risk
- **NoteBuilderService.UpdateObjectiveAsync()** (Lines 43-47)
  ```csharp
  note.Objective.Rom = rom?.ToList() ?? note.Objective.Rom;
  ```
  **Issue**: If `note.Objective` is null, this will throw NullReferenceException.

### 2. Database/Entity Framework Related Issues

#### High Risk
- **No Exception Handling in Service Methods**
  ```csharp
  public async Task<CheckInMessageLog> EnqueueCheckInAsync(...)
  {
      using var db = await _factory.CreateDbContextAsync();
      // ... database operations without error handling
      await db.SaveChangesAsync();
  }
  ```
  **Issue**: Database connection failures, constraint violations, or concurrency conflicts will cause unhandled exceptions.

- **ApplicationDbContext.SaveChangesAsync()** (Lines 19-28)
  ```csharp
  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
      // ... audit logic
      return base.SaveChangesAsync(cancellationToken);
  }
  ```
  **Issue**: No error handling for database constraint violations or connection failures.

#### Medium Risk
- **Seeder Program** (Lines 18-46)
  ```csharp
  await using var db = new ApplicationDbContext(options);
  await db.Database.EnsureCreatedAsync();
  ```
  **Issue**: No error handling for database initialization failures.

### 3. Type Conversion/Parsing Issues

#### Medium Risk
- **JSON Serialization Without Validation**
  ```csharp
  public string JsonSchema { get; set; } = "{}";
  public string AnswersJson { get; set; } = "{}";
  ```
  **Issue**: No validation that these strings contain valid JSON.

- **Numeric String Properties**
  ```csharp
  public string? PainSeverity0to10 { get; set; }
  ```
  **Issue**: No validation that string represents valid numeric range (0-10).

### 4. Collection/Index Access Issues

#### Medium Risk
- **Dictionary Access Without Validation**
  ```csharp
  public static readonly Dictionary<string, List<string>> ExerciseLibrary = new()
  {
      ["Neck"] = new() { "Chin tucks", "Cervical isometrics", "Scapular retraction with band" },
      // ...
  };
  ```
  **Issue**: Accessing non-existent keys will throw KeyNotFoundException.

### 5. Data Validation Issues

#### High Risk
- **No Input Validation for Critical Fields**
  ```csharp
  public string? Email { get; set; }
  public string? MobilePhone { get; set; }
  public DateTime? DateOfBirth { get; set; }
  ```
  **Issue**: No format validation for email, phone, or logical validation for dates.

- **Measurement Values Without Range Validation**
  ```csharp
  public int? MeasuredDegrees { get; set; }
  public int? NormalDegrees { get; set; }
  ```
  **Issue**: Could accept negative or unrealistic values.

#### Medium Risk
- **String Length Validation Only at Database Level**
  ```csharp
  e.Property(p => p.FirstName).HasMaxLength(60).IsRequired();
  ```
  **Issue**: Business logic doesn't validate before database operations, causing exceptions.

### 6. Date/Time Related Issues

#### Medium Risk
- **No Date Range Validation**
  ```csharp
  public DateTime? DateOfBirth { get; set; }
  public DateTimeOffset ScheduledStart { get; set; }
  ```
  **Issue**: Could accept future birth dates or past appointment dates.

- **Timezone Handling**
  ```csharp
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  ```
  **Issue**: Mixed use of DateTime and DateTimeOffset could cause timezone confusion.

### 7. Concurrency Issues

#### Medium Risk
- **No Optimistic Concurrency Control**
  ```csharp
  public abstract class Entity
  {
      // No RowVersion or ConcurrencyToken
  }
  ```
  **Issue**: Multiple users could modify same entity simultaneously without conflict detection.

### 8. Resource Management Issues

#### Low Risk
- **PDF Generation Without Error Handling**
  ```csharp
  public byte[] RenderSimple(string title, string body)
  {
      return Document.Create(container => { ... }).GeneratePdf();
  }
  ```
  **Issue**: SkiaSharp or QuestPDF exceptions not handled.

### 9. Performance Issues

#### Medium Risk
- **Unbounded Database Queries**
  ```csharp
  return await db.Patients.AsNoTracking()
      .Where(p => EF.Functions.Like((p.FirstName + " " + p.LastName).ToLower(), like))
      .Take(take).ToListAsync();
  ```
  **Issue**: While `Take()` is used, the `like` pattern could still be expensive on large datasets.

## Test Infrastructure Issues

#### High Risk
- **Missing Type in Tests**
  ```csharp
  var factory = new PooledDbContextFactory<ApplicationDbContext>(options);
  ```
  **Issue**: `PooledDbContextFactory<>` not found, causing build failures.

## Recommendations

### Immediate Fixes Required
1. Add proper exception handling to all service methods
2. Implement input validation for all user-provided data
3. Add range validation for numeric fields
4. Fix test infrastructure by adding required dependencies

### Medium Priority
1. Implement optimistic concurrency control
2. Add comprehensive data validation attributes
3. Standardize on DateTimeOffset usage
4. Add bounds checking for all collection operations

### Long Term
1. Implement comprehensive logging and monitoring
2. Add performance metrics and query optimization
3. Implement proper error boundaries in the UI layer
4. Add integration tests for error scenarios