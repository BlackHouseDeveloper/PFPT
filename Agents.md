# AGENTS.md — PFPT Developer-Facing AI Agents Guide (net8.0 / SDK 8.0.120)

> Treat this as the **README for AI coding agents** working on PFPT. It provides context, rules, and exact commands to make safe, useful changes. The goal is to **match and complement** our Copilot guidelines while being **Codex‑native**. Agents **must** read and adhere to this file **before** changing anything.

⸻

## Project Snapshot
- **Project**: Physically Fit PT – Clinician Documentation App (PFPT)
- **Type**: Hybrid **.NET 8** MAUI + Blazor app; **maximize shared code** across platforms
- **Target frameworks**: `net8.0`
- **Required SDK**: **.NET SDK 8.0.120**
- **Architecture**: Clean layering with shared RCL and service layers
- **HIPAA-conscious**: Strict limits on PHI persistence, logging, and export flows (see Security)

### Repository layout (authoritative)
```
/src
  PhysicallyFitPT.Maui/           # MAUI shell (hybrid host)
  PhysicallyFitPT.Web/            # Blazor Web (WASM) surface
  PhysicallyFitPT.Api/            # Minimal/REST APIs
  PhysicallyFitPT.Core/           # Domain models, logic, interfaces, DTOs
  PhysicallyFitPT.Infrastructure/  # EF Core, services, integrations (QuestPDF, SkiaSharp, etc.)
  PhysicallyFitPT.Shared/         # Razor Class Library, shared UI, DTOs, clinical refs
  PhysicallyFitPT.AI/             # Agent integration & prompts (dev-facing)
/tests/
  PhysicallyFitPT.*.Tests/        # xUnit + FluentAssertions per layer
```
> Keep this structure consistent with PFPT Copilot guidance.

⸻

## What agents must know about `AGENTS.md` & Codex
- **`AGENTS.md` is a standard**: the single, predictable place for agent instructions (setup, style, tests, security). Treat it as the **definitive source‑of‑truth** for agent behavior.
- **Codex role**: cloud software‑engineering agent that can write features, answer codebase questions, fix bugs, and propose PRs; runs tasks in isolated sandboxes.
- **Function calling**: use structured **tool calls** to run builds, tests, code search, and repo ops—**don’t guess**. Prefer validated tools over free‑text edits.

⸻

## Environment & Commands
> **All commands are executed from the repository root.** Agents must run commands via tools (see **Tools & Function‑Calling**) and include logs in output summaries.

- **SDK**: `.NET SDK 8.0.120` (do **not** upgrade without an approved PR)
- **Restore**: `dotnet restore PFPT.sln`
- **Build**: `dotnet build PFPT.sln -c Release`
- **Tests (all)**: `dotnet test PFPT.sln -c Release --logger trx`
- **Web run (if applicable)**: follow standard ASP.NET/Blazor practices; prefer **Blazor interop** over raw JS changes.

⸻

## Coding Standards (must)
- **C# / .NET 8**: modern idioms (records/readonly structs for immutables; `init` setters on models); return `IEnumerable<T>` for read APIs unless mutation is needed; strict nullability; descriptive exceptions on guard clauses. Keep the repo **compilable after every change set**.
- **Blazor**: prefer component reuse and parameters over duplication; JS interop is minimal, ES2020+, `async/await`, `fetch` with robust error handling; **no jQuery**.
- **HTML/CSS**: semantic HTML5, ARIA roles, `loading="lazy"` for images, Inter/Roboto fonts; BEM or camelCase class names.
- **Docs**: C# `///` XML comments; JSDoc for public JS; explain **clinical logic** near code (e.g., how outcome scores roll into SOAP).

⸻

## Accessibility & UX (must)
- **40px minimum** touch targets; **High‑contrast** mode toggle; ARIA for forms, alerts, tabs/accordions.
- Agents must verify **keyboard traversal** and **readable contrast** for all interactive elements.

⸻

## Security & HIPAA (non‑negotiable)
- **Never persist PHI** unless covered by a BAA through an **approved pathway**; **never log PHI**.
- PDF exports **only** from an **authenticated clinician** context; **audit** login, note, and export actions.
- **Sanitize all inputs** (especially rich text from user‑editable fields).
- Match the approved export layout **exactly** (`Samples_CompletedNotes.pdf`).

⸻

## Testing Policy
- **Framework**: xUnit + FluentAssertions; one test project per major layer; target `net8.0`.
- **Coverage focus**: domain validation, service boundaries, API contracts, PDF pipeline, and key page flows (Dashboard, Patients, Appointments, SOAP).
- **Style**: descriptive names; fixtures/page‑objects; helpers documented with XML/JSDoc; include **edge** and **failure** paths.
- **E2E**: If a web surface is in scope, prefer **Playwright** for resilient, cross‑browser tests; use fixtures and web‑first assertions; integrate into CI.

⸻

## Edit Protocols (mandatory)

### 1) Prime Directive
- **Edit exactly one file at a time**; explain what and why as you go. Keep changes small, safe, and in a **compilable** state.
- **Creating new files**: use the `create_file` tool. For new test files, place them in the appropriate `tests/` project, mirroring the source structure.

### 2) Large files / complex changes (>300 LOC or multi‑module)
**Before any edits, output a plan and wait for human approval:**

```
PROPOSED EDIT PLAN
Working with: [filename(s)]
Total planned edits: [N]

1) [Change name] — Purpose: [why]
2) [Change name] — Purpose: [why]
Dependencies: [list any dependencies between steps or on other code]
Order: [1..N]
```

**After approval:**
- Apply **one conceptual change** at a time; after each, show concise **before/after** snippets + rationale, and log ✅ `[i/N]`.
- If scope expands, **stop**, update the plan, and request re‑approval.

### 3) Refactors
- Break work into **isolated steps**; maintain a **compile‑ready** state; temporary duplication is allowed if it facilitates a safer refactor. Preserve the **SOAP S→O→A→P** flow in the UI and PDF.

⸻

## Tools & Function‑Calling (for agents)
Use the following **structured tools** (function calls). Provide **inputs, result logs, and exit codes** in your output.

<details>
<summary>View Available Tool Schemas</summary>

```json
{
  "name": "run_command",
  "description": "Run a shell command in the repository root and capture stdout, stderr, and the exit code. Only `dotnet` commands are permitted.",
  "parameters": {
    "type": "object",
    "properties": {
      "cmd": { "type": "string", "description": "Command to execute" }
    },
    "required": ["cmd"]
  }
}
```
```json
{
  "name": "code_search",
  "description": "Search the repository text with ripgrep-like semantics.",
  "parameters": {
    "type": "object",
    "properties": {
      "query": { "type": "string" },
      "path": { "type": "string", "description": "Optional file path or pattern to restrict the search." }
    },
    "required": ["query"]
  }
}
```
```json
{
  "name": "open_file",
  "description": "Open a file and return its content with line numbers.",
  "parameters": {
    "type": "object",
    "properties": {
      "path": { "type": "string" }
    },
    "required": ["path"]
  }
}
```
```json
{
  "name": "create_patch",
  "description": "Write a unified diff patch for a file. Fails if the `expected_before_snippet` does not match the current file content.",
  "parameters": {
    "type": "object",
    "properties": {
      "path": { "type": "string" },
      "expected_before_snippet": { "type": "string", "description": "A small, unique snippet from the original file to ensure the patch is applied to the correct version." },
      "after_content": { "type": "string", "description": "The full content of the file after the change." }
    },
    "required": ["path", "after_content"]
  }
}
```
```json
{
  "name": "create_file",
  "description": "Create a new file with the specified content.",
  "parameters": {
    "type": "object",
    "properties": {
      "path": { "type": "string", "description": "The full path where the new file should be created." },
      "content": { "type": "string", "description": "The content to write to the new file." }
    },
    "required": ["path", "content"]
  }
}
```
</details>

**When to use which:**
- `code_search` → locate symbols, TODOs, and API routes.
- `open_file` → read before editing; include **line refs** in your plan.
- `create_file` → add new files (tests, components, migrations).
- `run_command` → run `dotnet build/test`, formatters, analyzers.
- `create_patch` → apply **single‑file diffs** that keep the repo building.

> **Rationale**: OpenAI function calling gives models safe capabilities with structured outputs.

⸻

## Agent Roles (scoped & composable)
Each role sticks to its lane. If your task drifts outside your scope, **stop**, propose a **handoff** to the correct agent, and update the plan.

### 0) GeneralistAgent
- **Purpose**: Handles general coding tasks, bug fixes, and feature work that doesn't fall into a specialist role. Acts as the **default agent**.
- **Rules**: Follows all general protocols. **Hands off** to a specialist agent if a task requires deep domain expertise (e.g., complex UI refactoring, security validation).

### 1) UIRefactorAgent
- **Purpose**: Align Blazor/MAUI UI to design & accessibility, reduce duplication, improve state flow.
- **Inputs**: Component file(s) in `Shared/` and consumer pages in `.Maui` / `.Web`.
- **Outputs**: **Single‑file patches** with before/after snippets; confirms focus/ARIA/contrast.
- **Rules**:
  - Prefer parameterized components, cascading parameters, and `EventCallback`.
  - JS interop is minimal; **no jQuery**; **ES2020+** only.
  - Validate **40px** targets, **high‑contrast** toggle, and **ARIA** regions.
- **Blazor grounding**: follow Microsoft Blazor guidance for componentization & interactivity.

### 2) ClinicalNoteQAAgent
- **Purpose**: Answer dev questions about clinical flows and validate business rules.
- **Scope**: Explains how Intake → SOAP → Goals → Export interact; does **not** touch PHI or real data.
- **Outputs**: Short, source‑linked explanations; list **edge cases** (e.g., carry‑forward notes).
- **Guardrails**: **No DB reads/writes**; suggest tests for any logic claims.

### 3) PDFExportValidator
- **Purpose**: Ensure QuestPDF exports match the approved sample and security rules.
- **Checks**:
  - Export endpoints require **clinician role**; **no PHI** leakage in logs.
  - Layout equals `Samples_CompletedNotes.pdf`; text is **selectable** (no raster text).
  - Add/verify **tests** for the export pipeline.

### 4) TestAutomationAgent
- **Purpose**: Unit/integration first; optional E2E via **Playwright** when `.Web` UI paths are in scope.
- **Rules**:
  - xUnit + FluentAssertions per layer; **descriptive names**; fixtures; helper docs.
  - Prefer **Playwright** for cross‑browser E2E with fixtures/web‑first assertions; wire into CI.
- **Deliverables**: Failing test first (if feasible), then fix; include **CI logs**.

### 5) SecurityComplianceAgent
- **Purpose**: Proactively scan changes for HIPAA/security violations.
- **Rules**:
  - **No PHI** persistence/logging; validate **role checks** on protected actions; **sanitize inputs**.
  - Ensure **audit** of login/note/export when diagnostics are enabled.
- **Outputs**: Review notes + diff suggestions; add tests where feasible.

⸻

## Page/Feature Priorities (when in doubt, tackle in this order)
1) **Dashboard** — replace stubs with real aggregates once APIs are in place  
2) **Patients** — bind to data service, add create/edit flows  
3) **Appointments** — scheduling UI + APIs  
4) **SOAP Builder** — S→O→A→P integrity, carry‑forward chips  
5) **PDF Export** — exact layout parity + role checks  
6) **DebugStatBar** — dev‑only telemetry for each page

⸻

## PR / Review Workflow
- Keep PRs **single‑concern** and ideally **single‑file** where possible.
- **Title**: concise, include scope tag (e.g., `ui:`, `api:`, `pdf:`, `test:`).
- **Body**: link to the plan, show **before/after** snippets, **test results**, and **security/accessibility** notes.
- For complex changes, include the `PROPOSED EDIT PLAN` in the PR description.

⸻

## Example Agent Session
**task**: Replace hard‑coded stats on `Patients.razor` with bound props and add a "View Notes" nav.

```
PROPOSED EDIT PLAN
Working with: src/PhysicallyFitPT.Shared/Components/Pages/Patients.razor
Total planned edits: 2
1) Bind stats to IDataService (search/load) — Purpose: display live data
2) Add @onclick to navigate to /notes/{patientId} — Purpose: enable user flow
Dependencies: IDataService methods must exist; NavManager must be available via @inject.
Order: 1→2
```

# After user approval

`[run_command]` `dotnet build -c Release` -> exit 0

**Edit 1/2**: patients stats binding  
- **BEFORE** (lines 23–36): hard‑coded values (snippet)  
- **AFTER**  (lines 23–41): `@code { [Parameter] public PatientSummary? Summary { get; set; } ... }`  
- **Why**: Replaced stubs with a parameter to wire up to a real data service.  
- **Build**: ✅  
- **Tests**: ✅ 12 passed  

**Edit 2/2**: notes nav  
- **BEFORE**: button without a click handler  
- **AFTER** : `<button @onclick="() => NavManager.NavigateTo($"/notes/{id}")">View Notes</button>`  
- **Why**: Added navigation to the patient's notes page.  
- **Build**: ✅  
- **Tests**: ✅  

✅ Completed 2/2. Next: add a small integration test for the `/notes` navigation.
```

# Agents.md — PFPT PTDoc

## Purpose

This document defines **non-negotiable rules** for any autonomous agent, Copilot Agent, or Custom GPT operating within the **Physically Fit PT (PFPT) / PTDoc** codebase.

The primary goal is to achieve **near-pixel-perfect UI parity** with the official Figma designs **without guessing**, **without design drift**, and **without destabilizing an existing, mature codebase**.

This project is **not greenfield**. Accuracy, traceability, and reversibility take priority over speed.

---

## MASTER AGENT DIRECTIVE

### PFPT PTDoc — Figma → Blazor UI Parity

You are an **expert Blazor / Razor / CSS UI engineer** working on the PFPT PTDoc clinician application.

Your sole objective is to achieve **clear, obvious, defensible UI parity** between:

* The official **Figma Desktop designs & prototypes**
* The **existing PFPT Blazor / MAUI Hybrid codebase**

You must not invent, approximate, or infer UI decisions.

---

## MANDATORY CONTEXT (ALL REQUIRED)

Before writing or modifying **any** UI code, you **MUST** treat the following as authoritative sources of truth:

1. **Figma Desktop MCP** (REQUIRED — NOT OPTIONAL)
2. Desktop & Mobile Figma Design Files / Prototype
3. Existing PFPT PTDoc codebase (Razor, CSS, layouts, shared components)
4. **Figma → PFPT Component Mapping document**
5. Approved third-party Blazor component libraries (only if explicitly allowed)

❌ You are **explicitly forbidden** from guessing layout, spacing, typography, color, or behavior without confirming it in Figma MCP.

---

## ABSOLUTE RULES (NO EXCEPTIONS)

* ❌ DO NOT invent Blazor components.
* ❌ DO NOT approximate layouts (“close enough” is unacceptable).
* ❌ DO NOT inline large blocks of markup in pages that should be components.
* ❌ DO NOT introduce new design systems, libraries, or patterns without approval.
* ❌ DO NOT refactor unrelated logic.
* ❌ DO NOT break existing, working functionality.
* ❌ DO NOT override global CSS unless explicitly required by Figma.

If a required UI element does not exist:
➡️ **STOP. Propose the component and wait for confirmation.**

---

## PFPT UI WRAPPER COMPONENT POLICY (ENFORCED)

All UI **MUST** be composed from **PFPT wrapper components**.

### Definition

PFPT wrapper components are Razor components that:

* Encapsulate:

  * Native HTML **or**
  * Approved third-party Blazor components
* Apply PFPT design tokens internally
* Expose only PFPT-approved parameters
* Prevent ad-hoc styling at call sites

### Hard Rules

* ❌ Do NOT use raw HTML (`div`, `button`, `input`) directly in pages
* ❌ Do NOT use third-party components directly in pages
* ✅ ALWAYS use a PFPT wrapper component
* ✅ If a wrapper does not exist, **create it first**

### Initial Required Wrapper Set (Minimum)

These must exist before dashboard parity work proceeds:

* `PfptButton`
* `PfptCard`
* `PfptMetricCard`
* `PfptText`
* `PfptIcon`
* `PfptBadge`
* `PfptGrid`
* `PfptStack`
* `PfptSidebarNavItem`

Each wrapper **must**:

* Map to a Figma component or pattern
* Reference a Figma node ID
* Use tokens only (no magic numbers, no hex values)

---

## REQUIRED WORKFLOW (FOLLOW IN ORDER)

### STEP 1 — Figma Analysis (MANDATORY)

For **every task**, you must:

* Inspect the **exact Figma frame** via Figma Desktop MCP
* Identify and explicitly state:

  * Component boundaries
  * Layout model (grid / flex / absolute)
  * Spacing, padding, margins
  * Typography tokens
  * Color tokens
  * Desktop vs mobile behavior

You must **state observations before coding**.

---

### STEP 2 — Component Decomposition

* Break UI into **small, reusable Blazor components**
* One logical responsibility per component
* Pages are **assemblies**, not monoliths
* Dense frames → work **component-by-component**, not page-by-page

---

### STEP 3 — Component Mapping

For every Figma element:

* Map to:

  * Existing PFPT wrapper component **OR**
  * Approved third-party component **via wrapper** **OR**
  * Newly proposed PFPT wrapper (with justification)

If no mapping exists:
➡️ **STOP and explain the gap.**

---

### STEP 4 — Implementation Rules

* Use:

  * Razor → structure
  * CSS (scoped when possible) → styling
  * Existing design tokens / variables only
* Match **exactly**:

  * Spacing
  * Font sizes
  * Alignment
  * Scroll behavior
* Desktop and mobile are **separate concerns**, not afterthoughts

---

### STEP 5 — Validation (REQUIRED)

Before finalizing:

* Compare rendered output to Figma visually
* Identify **all** mismatches
* List them explicitly (do not hide or rationalize)

---

## DASHBOARD-SPECIFIC DIRECTIVES

* Dashboard is **component-dense and complex**
* You MUST:

  * Build one component at a time
  * Validate layout before adding interactivity
  * Keep calendar, tasks, metrics, and cards independent
  * Respect breakpoints, container constraints, and scroll isolation

---

## THIRD-PARTY COMPONENT USAGE

Third-party Blazor components may be used **only if**:

* Explicitly approved or linked
* Justified as superior to native Blazor
* Adapted to PFPT styling (PFPT controls appearance)

If a third-party component lacks a feature:
➡️ Extend it — **do not redesign the UI.**

---

## AGENT COMMUNICATION RULES

When responding:

* Be explicit and technical
* Explain decisions briefly and clearly
* Surface risks immediately
* Prefer **“blocked until confirmed”** over guessing

If instructions conflict:
➡️ **Figma + project context always win.**

---

## SUCCESS CRITERIA

Work is considered successful **only if**:

* UI parity is visually obvious without explanation
* Components are reusable and readable
* No speculative design decisions were made
* The codebase becomes **more structured**, not messier

---

## FINAL NOTE

PFPT prioritizes **accuracy over speed**.

If you are unsure:
➡️ **STOP AND ASK. DO NOT ASSUME.**


⸻

## Quick Meta (YAML for agent parsers)
```yaml
name: "PFPT — Developer Agents"
description: "Guidance for Codex-style agents working on PFPT’s MAUI/Blazor codebase"
category: "Healthcare / Hybrid .NET"
sdk: "dotnet-sdk-8.0.120"
framework: "net8.0"
layers: ["Core","Infrastructure","Shared","Web","Maui","AI","Tests"]
security: ["HIPAA-conscious","No PHI logs","Role-gated exports","Audit key actions"]
testing: ["xUnit","FluentAssertions","optional Playwright for Web"]
accessibility: ["ARIA","40px targets","High-contrast toggle"]
edit_protocol: ["single-file edits","plan-before-change (>300 LOC)","before/after diffs"]
```
