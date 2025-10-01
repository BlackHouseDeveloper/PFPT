# PFPT Project Structure

## Overview

PFPT follows Clean Architecture principles with clear separation of concerns:

```
{{PROJECT_TREE}}
```

## Layer Dependencies

- **Core**: No dependencies (domain entities)
- **Infrastructure**: Depends on Core (data access, external services)
- **Shared**: Common utilities and shared models
- **Maui**: Platform-specific UI implementation
- **Web**: Blazor WebAssembly client

## Data Flow

1. UI Layer (Maui/Web) → Services
2. Services → Repository Interfaces  
3. Infrastructure → Entity Framework → SQLite
4. PDF Generation → QuestPDF → File System

## Healthcare Architecture

- **Clinical Workflow Services**: Patient assessment and documentation
- **HIPAA Compliance Layer**: Data encryption and audit logging
- **PDF Generation**: Clinical reports and patient documentation
- **Accessibility Layer**: WCAG 2.1 compliance for inclusive healthcare

_Auto-generated: {{GENERATION_DATE}}_