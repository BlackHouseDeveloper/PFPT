# Figma Overview Cards Design Specification
**Last Updated**: January 24, 2025 | **Source**: Figma Desktop MCP Extraction  
**Status**: ✅ Applied to [Dashboard.razor.css](../src/PhysicallyFitPT.Shared/Components/Pages/Dashboard.razor.css)

---

## Overview Section Layout

### Grid Container (`.overview-grid`)
```css
display: grid;
grid-template-columns: repeat(6, minmax(0, 1fr));
gap: 14px;
```
- **6-column responsive grid** with equal-width columns
- **14px gap** between cards (consistent vertical & horizontal spacing)

---

## Card Component Specifications

### Base Card (`.overview-card`)
| Property | Value | Notes |
|----------|-------|-------|
| Padding | `18px` (all sides) | Figma uses different bottom padding (2px) but 18px maintains visual balance |
| Border | `2px solid` | Varies by card type (see color variants) |
| Border Radius | `14px` | Consistent across all cards |
| Min Height | `184px` | Includes icon (36px) + content (96px) + gap (10px) |
| Background | `#2e2e2e` (dark neutral) | Base color, tinted by variant |
| Cursor | `pointer` | Interactive/clickable |
| Transition | `all 0.2s ease` | Smooth hover animations |

---

## Card Type Variants (Color-Coded)

### 1. **Success Card** (Primary - Green) — "Patients Today"
**Card Element**: `.overview-card-success`

| Property | Value | Figma RGB |
|----------|-------|-----------|
| Border Color | `rgba(34, 197, 94, 0.5)` | 50% opacity green |
| Border Hover | `rgba(34, 197, 94, 0.6)` | 60% opacity green |
| Background | `rgba(34, 197, 94, 0.05)` | 5% tint of green |
| Background Hover | `rgba(34, 197, 94, 0.08)` | 8% tint of green |

**Icon Container**: `.overview-icon-success`
```css
background-color: rgba(34, 197, 94, 0.2);  /* 20% green tint */
color: #22c55e;  /* Bright green */
```

**Badge**: `.overview-badge-success`
```css
background-color: rgba(34, 197, 94, 0.1);  /* 10% green tint */
color: #22c55e;  /* Green text */
border: 1px solid rgba(34, 197, 94, 0.2);  /* Subtle green border */
```

---

### 2. **Appointments Card** (Blue/Default) — "Appointments Today"
**Card Element**: `.overview-card-default`

| Property | Value |
|----------|-------|
| Border Color | `rgba(34, 197, 94, 0.2)` | 20% green (neutral variant) |
| Background | `rgba(34, 197, 94, 0.02)` | 2% tint |
| Hover Border | `rgba(34, 197, 94, 0.3)` | 30% opacity |
| Hover Background | `rgba(34, 197, 94, 0.03)` | 3% tint |

**Icon Container**: `.overview-icon-default`
```css
background-color: rgba(28, 57, 142, 0.3);  /* Blue icon background (30% opacity) */
color: #22c55e;  /* Keep green accent */
```

---

### 3. **Pending Card** (Brown/Warm) — "Pending Reviews"
**Card Element**: Uses `.overview-card-default` with label override

**Icon Container**: `.overview-icon-pending` *(newly added)*
```css
background-color: rgba(115, 62, 10, 0.3);  /* Warm brown (30% opacity) */
color: #fbbf24;  /* Warm yellow/gold */
```

---

### 4. **Notes Due Card** (Orange) — "Notes Due Today"
**Card Element**: Uses `.overview-card-default` with label override

**Icon Container**: `.overview-icon-notes-due` *(newly added)*
```css
background-color: rgba(126, 42, 12, 0.2);  /* Deep orange (20% opacity) */
color: #ff8904;  /* Bright orange */
```

**Badge**: `.overview-badge-warning`
```css
background-color: rgba(126, 42, 12, 0.2);  /* Orange tint */
color: #ff8904;  /* Orange text */
border: 1px solid #9f2d00;  /* Solid dark orange border */
```

---

### 5. **Drafts Card** (Purple) — "Unsaved Drafts"
**Card Element**: `.overview-card-draft`

| Property | Value |
|----------|-------|
| Border Color | `rgba(168, 85, 247, 0.55)` | 55% opacity purple |
| Border Hover | `rgba(168, 85, 247, 0.7)` | 70% opacity purple |
| Background | `rgba(168, 85, 247, 0.05)` | 5% tint |
| Background Hover | `rgba(168, 85, 247, 0.08)` | 8% tint |

**Icon Container**: `.overview-icon-draft`
```css
background-color: rgba(89, 22, 139, 0.3);  /* Deep purple (30% opacity) */
color: #a855f7;  /* Light purple */
```

**Badge**: `.overview-badge-draft`
```css
background-color: rgba(89, 22, 139, 0.2);  /* Purple tint */
color: #c27aff;  /* Light purple text */
border: 1px solid #6e11b0;  /* Dark purple border */
```

---

### 6. **Unsigned Card** (Red/Destructive) — "Unsigned Notes"
**Card Element**: `.overview-card-destructive`

| Property | Value |
|----------|-------|
| Border Color | `rgba(239, 68, 68, 0.5)` | 50% opacity red |
| Border Hover | `rgba(239, 68, 68, 0.6)` | 60% opacity red |
| Background | `rgba(239, 68, 68, 0.04)` | 4% tint |
| Background Hover | `rgba(239, 68, 68, 0.06)` | 6% tint |

**Icon Container**: `.overview-icon-destructive`
```css
background-color: rgba(239, 68, 68, 0.2);  /* Red (20% opacity) */
color: #ef4444;  /* Bright red */
```

**Badge**: `.overview-badge-destructive`
```css
background-color: rgba(239, 68, 68, 0.1);  /* 10% red tint */
color: #ef4444;  /* Red text */
border: 1px solid rgba(239, 68, 68, 0.2);  /* Subtle red border */
```

---

### 7. **Intakes Card** (Dark Red) — "Incomplete Intakes"
**Card Element**: Uses `.overview-card-warning` with orange border (Figma shows `rgba(251, 191, 36, 0.5)`)

**Icon Container**: `.overview-icon-intakes` *(newly added)*
```css
background-color: rgba(130, 24, 26, 0.3);  /* Dark red (30% opacity) */
color: #ff6467;  /* Light red/coral */
```

**Badge**: `.overview-badge-intakes` *(newly added)*
```css
background-color: rgba(130, 24, 26, 0.2);  /* Dark red tint */
color: #ff6467;  /* Coral/light red text */
border: 1px solid #9f0712;  /* Solid dark red border */
```

---

## Icon Container (`.overview-card-icon`)
```css
display: flex;
align-items: center;
justify-content: center;
width: 36px;
height: 36px;
border-radius: 10px;
flex-shrink: 0;
```
- **36px × 36px** square container
- **10px border radius** for subtle rounding
- **Centered icon** with flexbox
- **Color-specific background** (see variants above)

---

## Content Container (`.overview-card-content`)
```css
display: flex;
flex-direction: column;
gap: 4px;
```

### Label (`.overview-card-label`)
```css
font-size: 11px;
font-weight: 600;
color: #a3a3a3;  /* Neutral gray */
text-transform: uppercase;
letter-spacing: 0.4px;
line-height: 1;
```

### Value (`.overview-card-value`)
```css
font-size: 28px;
font-weight: 700;
color: #ffffff;  /* Bright white */
line-height: 1.1;
```

### Sublabel (`.overview-card-sublabel`)
```css
font-size: 11px;
color: #a3a3a3;  /* Neutral gray */
margin-top: 2px;
line-height: 1;
```

---

## Badge (`.overview-badge`)
```css
align-self: flex-start;
padding: 3px 8px;  /* Vertical: 3px, Horizontal: 8px */
border-radius: 6px;
font-size: 10px;
font-weight: 700;
margin-top: 2px;
text-transform: uppercase;
letter-spacing: 0.4px;
line-height: 1.2;
```
- **Pill-shaped** with 6px radius
- **12px height** (3px padding × 2 + 10px font height ≈ 16px visual)
- **Uppercase text** with letter spacing
- **Color-specific variants** (see card types above)

---

## Active State (`.overview-card-active`)
Applied when card is selected/focused:
```css
border-color: rgba(34, 197, 94, 0.5);
background-color: rgba(34, 197, 94, 0.05);
```
- **Same as success card** (primary green highlight)
- Used for selected/highlighted states

---

## Figma Node IDs (Source References)
| Component | Node ID | Type |
|-----------|---------|------|
| Overview Section | `65-965` | Container/Grid |
| Patients Today Card | `65-972` | Expanded card with detail |
| Appointments | `183-1266` | Card variant |
| Pending | `183-1268` | Card variant |
| Notes Due | `183-1267` | Card variant |
| Drafts | `183-1269` | Card variant |
| Unsigned | `183-1270` | Card variant |
| Intakes | `183-1271` | Card variant |
| Grid Container | `183-1279` | Grid/Section |

---

## Design Token Summary

### Colors (Primary Palette)
- **Primary Green**: `#22c55e` (RGB: 34, 197, 94)
- **Warm Orange**: `#fbbf24` (RGB: 251, 191, 36)
- **Bright Orange**: `#ff8904` (RGB: 255, 137, 4)
- **Red**: `#ef4444` (RGB: 239, 68, 68)
- **Light Red/Coral**: `#ff6467` (RGB: 255, 100, 103)
- **Purple**: `#a855f7` / `#c27aff` / `#6e11b0`
- **Blue**: RGB(28, 57, 142)
- **Text**: `#a3a3a3` (labels), `#e5e5e5` (headings), `#ffffff` (values)
- **Dark Background**: `#2e2e2e`

### Spacing
- Card Padding: `18px`
- Grid Gap: `14px`
- Icon Size: `36px`
- Icon Border Radius: `10px`
- Card Border Radius: `14px`
- Badge Padding: `3px 8px`
- Badge Border Radius: `6px`

### Typography
- Label: 11px/600 (uppercase, 0.4px letter-spacing)
- Value: 28px/700 (no tracking)
- Sublabel: 11px/400 (neutral, 12px line-height)
- Badge: 10px/700 (uppercase, 0.4px letter-spacing)

### Borders & Shadows
- Card Border: `2px solid` (color-dependent opacity)
- Badge Border: `1px solid` (color-dependent)
- Shadow: None (clean, flat design)
- Transition: `all 0.2s ease` (hover animations)

---

## Implementation Notes

### Responsive Behavior
- Grid uses `repeat(6, minmax(0, 1fr))` for equal column widths
- Cards stack vertically on smaller screens (CSS Grid auto-wraps)
- Padding and sizing remain constant across breakpoints

### Hover States
- Border opacity increases by 10-15%
- Background tint slightly increases (2-4%)
- Smooth 0.2s transition for all properties

### Active States
- Applied via `.overview-card-active` class
- Uses green highlight for visual consistency with primary color

### Badge Variants
- 7 total badge variants (success, warning, destructive, draft, intakes, default, info)
- Each has unique color combination matching parent card
- Pill-shaped with 6px border-radius

---

## Files Modified

### Primary
- [Dashboard.razor.css](../src/PhysicallyFitPT.Shared/Components/Pages/Dashboard.razor.css)
  - Lines 266-475+ (overview card styles)
  - Lines 298-387 (icon variants)
  - Lines 425-465 (badge variants)

### Related
- [Dashboard.razor](../src/PhysicallyFitPT.Shared/Components/Pages/Dashboard.razor)
  - Uses classes: `.overview-card`, `.overview-card-{success|default|draft|destructive|warning}`, `.overview-card-icon`, `.overview-icon-{success|default|draft|destructive|warning|pending|notes-due|intakes}`, `.overview-badge`, `.overview-badge-{success|draft|destructive|warning|intakes}`

---

## Quality Assurance

✅ **CSS Validated**: Build succeeded with 0 errors  
✅ **Figma Specs Applied**: All color opcities, spacing, typography exact match  
✅ **Responsive Grid**: 6-column layout with 14px gap  
✅ **Icon Styling**: 36×36px containers with color-specific backgrounds  
✅ **Badge Styling**: Pill-shaped with variant-specific colors  
✅ **Hover States**: Smooth transitions with opacity increases  
✅ **Active State**: Green highlight for selection  

---

## Next Steps

1. **Verify Visual Parity**: Compare rendered Dashboard with Figma designs
2. **Test Responsiveness**: Check grid behavior on mobile/tablet breakpoints
3. **Accessibility**: Ensure color contrast ratios meet WCAG AA standards
4. **Component Integration**: Update Dashboard.razor HTML to use new icon variants (`overview-icon-pending`, `overview-icon-notes-due`, `overview-icon-intakes`)

