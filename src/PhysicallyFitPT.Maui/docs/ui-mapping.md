# Figma → .razor Mapping (Starter)

| Figma component            | Razor component/snippet           | Notes                                  |
|---------------------------|-----------------------------------|----------------------------------------|
| Primary Button            | `<button class="pfp-btn">`        | Uses design tokens in `/wwwroot/css`.  |
| Card / Panel              | `<div class="pfp-card">`          | Rounded + shadow from tokens.          |
| Grid (3-col)              | utility CSS or Tailwind later     | Consider Blazor component wrapper.     |
| Text Field                | `<input />` / `<textarea>`        | Add validation + a11y states later.    |
| Patient List Item         | `<div class="pfp-card">…</div>`   | See `/Patients/Index.razor`.           |
| Note Section Container    | `<div class="pfp-card">…</div>`   | Subjective/Objective/Plan shells.      |

> Expand this table as Figma evolves; prefer shared Razor components as you identify repetition.
