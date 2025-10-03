🔧 PFPT Copilot Agent Operational Guidelines (v1.0)

🧭 Project Name: Physically Fit PT – Clinician Documentation App

🧠 Role: Copilot Agent (Developer-Facing)

📅 Version: October 2025

⸻

🟢 PRIME DIRECTIVE
	•	Edit only one file at a time (to prevent race conditions or git merge issues).
	•	Teach and explain each edit so that human developers understand your logic.
	•	All code must be HIPAA-conscious and compliant with internal app structure.
	•	All AI suggestions must be explainable and grounded in actual code/data context.

⸻

🏗️ LARGE FILE / COMPLEX CHANGE PROTOCOL

✅ PLANNING PHASE – Mandatory Before Code Changes

When editing files >300 lines or with complex logic (e.g., EF Core, SOAP generators, PDF pipelines):

## PROPOSED EDIT PLAN
Working with: [filename]
Total planned edits: [X]

1. [Change name] – Purpose: [why]
2. [Change name] – Purpose: [why]
...
Do you approve this plan? I will proceed with edit [#] after confirmation.

✅ EXECUTION PHASE

After user approval:
	•	Show “before” and “after” snippets for each edit
	•	Include a short explanation: what changed, why, and how it fits into PFPT
	•	After each change, log:
✅ Completed edit [X of Y]. Ready for next?

If you encounter new required changes during editing:
	•	❗ Stop
	•	📋 Update the edit plan
	•	🔁 Request confirmation before continuing

⸻

♻️ REFACTORING RULES
	•	Break work into logically isolated steps
	•	Maintain intermediate compile/run-ready state
	•	Use duplication temporarily if needed (e.g., parallel components for mobile vs desktop)
	•	Always preserve SOAP → S/O/A/P flow in UI and PDF

⸻

⚙️ TECHNOLOGY + CODING STANDARDS

✅ .NET / C# (.NET 8+)
	•	Use record or readonly struct where immutable
	•	Use init accessors for models
	•	Return IEnumerable<T> for service results unless mutation needed
	•	Respect PFPT Clean Architecture (Core, Infrastructure, Shared, Web, Maui)
	•	Follow StyleCop + Roslynator standards (auto-enforced via .editorconfig)
	•	Validate inputs, throw exceptions with descriptive messages

✅ JavaScript (Web Clients)
	•	Use ES2020+ (e.g., optional chaining, arrow functions, destructuring)
	•	Prefer async/await over .then() chains
	•	Avoid jQuery, var, and eval
	•	Prefer fetch() with robust error handling
	•	Scope all logic to Blazor interop needs

✅ HTML/CSS
	•	Semantic HTML5 with ARIA attributes
	•	loading="lazy" for images
	•	Ensure keyboard accessibility (WCAG 2.1 AA minimum)
	•	Use Inter and Roboto (Figma Style Guide)
	•	Class naming: use BEM or camelCase for scoped Blazor styles

⸻

✅ MAUI + Blazor App Architecture

Maintain PFPT scaffolding structure as per PFPT-Foundry.sh:

src/
  PhysicallyFitPT.Maui/         → Cross-platform MAUI shell
  PhysicallyFitPT.Web/          → Web Blazor WASM (limited scope)
  PhysicallyFitPT.Api/          → REST/Minimal APIs
  PhysicallyFitPT.Core/         → Domain models, logic
  PhysicallyFitPT.Infrastructure/ → EF Core, services, SkiaSharp, QuestPDF
  PhysicallyFitPT.Shared/       → DTOs, ClinicalRef libs, RCLs
  PhysicallyFitPT.AI/           → GPT agent logic
tests/
  PhysicallyFitPT.*.Tests/      → Unit/integration coverage

🧪 Tests: Use xUnit + FluentAssertions. Target net8.0. One test project per major layer.

⸻

✅ DOCUMENTATION + INLINE COMMENTS
	•	Domain/Service Layers: use XML comments or C# /// format
	•	JavaScript: use JSDoc for public functions
	•	Explain clinical logic clearly (e.g., “PN note requires comparison of outcome scores”)

Each component or function must include:

/// <summary>Short summary</summary>
/// <param name="...">...</param>
/// <returns>...</returns>


⸻

🛡️ SECURITY + COMPLIANCE
	•	Enforce HIPAA-conscious coding:
	•	Never persist PHI unless Azure BAA is confirmed
	•	PDF exports must only be triggered by clinician
	•	Audit all login, note, export actions
	•	Sanitize all user inputs (especially from form fields)
	•	Lock down PDF exports to match sample in Samples_CompletedNotes.pdf

⸻

🧪 TESTING & CI/CD GUIDANCE
	•	Use dotnet test + GitHub Actions for CI
	•	When modifying a logic/service layer:
	•	Add tests under /tests/
	•	Include edge cases and failure paths
	•	Trigger GitHub Copilot Agent via PR title tag: #copilot

⸻

⚠️ RATE LIMIT & SESSION SIZE
	•	If file size is too large (>3000 lines) or change scope is extensive:
	•	Break into parts across sessions
	•	Prioritize UI layout stubs, then data binding, then logic injection
	•	Notify user of planned cut points

⸻

✅ UI COMPONENT PRIORITY (match Figma Handoff)

Build or refactor these components first:

Component	Pages Used On
PatientCard	Dashboard, Export
ROMGrid, MMTGrid	SOAP → Objective
GoalComposer	SOAP → Assessment
ICDPicker, CPTPicker	SOAP → Plan
PDFDownloadBlock	Export PDF view
AdminPanel	User/role config, audit logs
DebugStatBar	Visible only in dev; shows page-level telemetry


⸻

✅ ACCESSIBILITY MANDATES (ALL PAGES)
	•	Minimum 40px touch targets
	•	High contrast mode toggle
	•	Use ARIA roles for all:
	•	Form regions
	•	Toast/alert messages
	•	Tab or accordion panels

⸻

🧾 FILE EDIT FORMAT EXAMPLE

## PROPOSED EDIT PLAN
Working with: `PatientCard.razor`
Total planned edits: 2

1. Replace hardcoded patient stats with dynamic props
   - Purpose: Allow live data from API via `@bind-Patient`
2. Add click event for "View Notes" → navigates to `/notes/{patientId}`
   - Purpose: Enable navigation flow from dashboard

Do you approve this plan? I will begin with Edit 1 after confirmation.


⸻

🧩 FLOW-SPECIFIC RULES

When editing features tied to clinical flow:

Feature	Rule
Intake	Use branching per condition; pull from ClinicalReference bundle
SOAP Builder	Maintain S → O → A → P order; carry-forward prior notes as chips
Goals Generator	Use ROM/MMT deficit + ICD code to auto-suggest
AI Summary Modal	Always editable; prompt shown above AI text
Export PDF	Must match Samples_CompletedNotes.pdf format
Audit Logs	All saves, edits, logins, exports must be captured if dev enabled
