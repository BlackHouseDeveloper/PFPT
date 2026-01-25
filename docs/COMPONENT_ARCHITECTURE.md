# Dashboard Component Architecture

## Visual Component Hierarchy

```
Dashboard.razor
├── Header Section
│   ├── Title: "Dashboard"
│   ├── Greeting: "Welcome back, Dr. Sarah Johnson • [Date]"
│   └── Notification Bell (with urgent indicator)
│
├── Error Banner (conditional)
│   ├── Icon: alert-circle
│   ├── Message + Subtitle
│   └── Retry Button
│
├── Quick Actions Section (2-column grid)
│   ├── Button 1: Add Patient (Green #22c55e)
│   │   ├── Icon Wrapper (48×48, dark background)
│   │   ├── Icon: user-plus
│   │   ├── Title: "Add Patient"
│   │   ├── Subtitle: "Create new patient profile"
│   │   └── Arrow Icon
│   │
│   └── Button 2: Send Intake (Blue #60a5fa)
│       ├── Icon Wrapper (48×48, dark background)
│       ├── Icon: send
│       ├── Title: "Send Intake"
│       ├── Subtitle: "Email intake form to patient"
│       └── Arrow Icon
│
├── Main Dashboard Grid (2 columns on desktop)
│   │
│   ├── Left Column (Main Content)
│   │   │
│   │   ├── Overview Section
│   │   │   ├── Section Title: "Overview" + activity icon
│   │   │   └── Overview Grid (6 columns → 3 → 2 → 1)
│   │   │       ├── Card 1: Patients Today (Green)
│   │   │       │   ├── Icon: users
│   │   │       │   ├── Label: "Patients Today"
│   │   │       │   ├── Value: 3
│   │   │       │   └── Badge: "Scheduled"
│   │   │       │
│   │   │       ├── Card 2: Appointments (Neutral)
│   │   │       │   ├── Icon: calendar
│   │   │       │   ├── Label: "Appointments"
│   │   │       │   ├── Value: 3
│   │   │       │   └── Sublabel: "Today"
│   │   │       │
│   │   │       ├── Card 3: Notes Due (Orange)
│   │   │       │   ├── Icon: file-text
│   │   │       │   ├── Label: "Notes Due"
│   │   │       │   ├── Value: 2
│   │   │       │   └── Badge: "Due Today"
│   │   │       │
│   │   │       ├── Card 4: Pending (Neutral)
│   │   │       │   ├── Icon: clipboard-list
│   │   │       │   ├── Label: "Pending"
│   │   │       │   ├── Value: 4
│   │   │       │   └── Sublabel: "In Progress"
│   │   │       │
│   │   │       ├── Card 5: Drafts (Purple)
│   │   │       │   ├── Icon: file-pen-line
│   │   │       │   ├── Label: "Drafts"
│   │   │       │   ├── Value: 1
│   │   │       │   └── Badge: "Unsaved"
│   │   │       │
│   │   │       └── Card 6: Unsigned (Red)
│   │   │           ├── Icon: lock
│   │   │           ├── Label: "Unsigned"
│   │   │           ├── Value: 3
│   │   │           └── Badge: "Need Signature"
│   │   │
│   │   ├── Precaution Alert (conditional, Orange)
│   │   │   ├── Icon: shield
│   │   │   ├── Title: "⚠️ [N] patients with special precautions"
│   │   │   └── Description: "Review precaution details before treatment"
│   │   │
│   │   ├── Unsigned Notes Alert (conditional, Red)
│   │   │   ├── Icon: lock
│   │   │   ├── Title: "🔒 Unsigned Notes: [N]"
│   │   │   ├── Description: "[N] note(s) require(s) clinician signature"
│   │   │   ├── Sign Button
│   │   │   └── Preview List (first 2 unsigned notes)
│   │   │       ├── Item 1: Patient + Icon
│   │   │       ├── Item 2: Patient + Icon
│   │   │       └── "+[N] more unsigned notes"
│   │   │
│   │   ├── Notifications Panel
│   │   │   ├── Panel Title: "Notifications & Alerts" + bell icon
│   │   │   ├── Badge: "[N] Urgent" (conditional)
│   │   │   └── Notification List
│   │   │       ├── Notification 1 (High Priority)
│   │   │       │   ├── Icon Wrapper (colored background)
│   │   │       │   ├── Title: "Incomplete Intake Form"
│   │   │       │   ├── Description: "Patient has not completed..."
│   │   │       │   ├── Patient Name + ID
│   │   │       │   ├── Due Date
│   │   │       │   ├── Timestamp: "2 min ago"
│   │   │       │   ├── Action Button: Resend
│   │   │       │   └── Dismiss Button
│   │   │       │
│   │   │       ├── Notification 2 (High Priority)
│   │   │       │   └── [Similar structure]
│   │   │       │
│   │   │       └── [Up to 5 notifications total]
│   │   │
│   │   └── Appointments Panel
│   │       ├── Panel Title: "Today's Appointments" + calendar icon
│   │       ├── View All Button
│   │       └── Appointment List
│   │           ├── Appointment 1: Sarah Johnson
│   │           │   ├── Patient Name
│   │           │   ├── Intake Badge: "✓ Intake Complete"
│   │           │   ├── PN Due Badge
│   │           │   ├── Time: "09:00"
│   │           │   ├── Duration: "45 min"
│   │           │   └── Patient ID
│   │           │
│   │           ├── Appointment 2: Michael Chen
│   │           │   └── [Similar structure]
│   │           │
│   │           └── Appointment 3: Emily Rodriguez
│   │               └── [Similar structure]
│   │
│   └── [End of Left Column]
│
│   ├── Right Sidebar (320px on desktop, hidden on mobile)
│   │   │
│   │   ├── Recent Notes Widget
│   │   │   ├── Header: "Recent Notes"
│   │   │   ├── View All Link
│   │   │   └── Notes List
│   │   │       ├── Note 1: Sarah Johnson
│   │   │       │   ├── Name + Status Badge
│   │   │       │   ├── Type: "Progress Note"
│   │   │       │   ├── Date: "11/10/2025"
│   │   │       │   └── Actions: View, Download
│   │   │       │
│   │   │       └── [Similar items for other notes]
│   │   │
│   │   ├── Activity Widget
│   │   │   ├── Header: "Recent Activity"
│   │   │   └── Activity List
│   │   │       ├── Activity 1: "SOAP note completed..." (2 min)
│   │   │       ├── Activity 2: "Appointment scheduled..." (15 min)
│   │   │       └── Activity 3: "Patient intake received..." (1 hr)
│   │   │
│   │   ├── Incomplete Intakes Widget
│   │   │   ├── Header: "Incomplete Intake Forms"
│   │   │   ├── Badge: "3"
│   │   │   ├── Intake Items
│   │   │   │   ├── Item 1: Lisa Martinez (HIGH)
│   │   │   │   │   ├── Name + Badge
│   │   │   │   │   ├── ID: "PT-2024-009"
│   │   │   │   │   ├── Progress Bar: 0%
│   │   │   │   │   └── Label: "Completion 0%"
│   │   │   │   │
│   │   │   │   ├── Item 2: Emily Rodriguez (MEDIUM)
│   │   │   │   │   ├── Progress Bar: 33%
│   │   │   │   │   └── Label: "Completion 33%"
│   │   │   │   │
│   │   │   │   └── Item 3: David Kim (LOW)
│   │   │   │       ├── Progress Bar: 16%
│   │   │   │       └── Label: "Completion 16%"
│   │   │   │
│   │   │   ├── Footer: "1 urgent • 1 medium • 1 low priority"
│   │   │   └── View All Intakes Link
│   │   │
│   │   └── Recently Edited POCs Widget
│   │       ├── Header: "Recently Edited POCs"
│   │       └── POC Items
│   │           ├── POC 1: Sarah Johnson (Active)
│   │           │   ├── Name + Badge: "active"
│   │           │   ├── Meta: "POC POC-2024-001 • 708 days ago"
│   │           │   ├── Stats Grid (4 columns)
│   │           │   │   ├── 📅 7/12 (visits)
│   │           │   │   ├── 📝 18/36 (notes)
│   │           │   │   ├── ✓ 2/3 (goals)
│   │           │   │   └── 🔤 2 (sessions)
│   │           │   └── Utilization Bar: 50%
│   │           │
│   │           └── POC 2: Michael Chen (Expiring)
│   │               ├── Stats & Utilization: 93%
│   │               └── [Similar structure]
│   │
│   └── [End of Right Sidebar]
│
├── Dialogs (Modals)
│   │
│   ├── Modal 1: Add Patient (conditional)
│   │   ├── Header: "Add New Patient"
│   │   ├── Close Button (X)
│   │   ├── Form Body
│   │   │   ├── First Name field
│   │   │   ├── Last Name field
│   │   │   ├── Email Address field
│   │   │   ├── Phone Number field
│   │   │   └── Date of Birth field
│   │   └── Actions
│   │       ├── Cancel Button
│   │       └── Add Patient Button (Green)
│   │
│   ├── Modal 2: Send Intake (conditional)
│   │   ├── Header: "Send Intake Form"
│   │   ├── Close Button (X)
│   │   ├── Form Body
│   │   │   ├── Select Patient dropdown
│   │   │   ├── Email Address field
│   │   │   ├── Phone Number field
│   │   │   └── Helper text: "We'll send an SMS reminder..."
│   │   └── Actions
│   │       ├── Cancel Button
│   │       └── Send Intake Form Button (Green)
│   │
│   └── Modal 3: View Note (conditional)
│       ├── Header: "[NoteType] - [PatientName]"
│       ├── Close Button (X)
│       ├── Note Body
│       │   ├── Section: SUBJECTIVE
│       │   │   └── Note text
│       │   ├── Section: OBJECTIVE
│       │   │   └── Note text
│       │   ├── Section: ASSESSMENT
│       │   │   └── Note text
│       │   └── Section: PLAN
│       │       └── Note text
│       └── [No actions - view only]
│
└── [End of Dashboard]
```

## CSS Class Reference

### Layout Classes
- `.dashboard-root` - Main container (flex column)
- `.dashboard-header` - Header section
- `.dashboard-content` - Main content area
- `.dashboard-grid` - 2-column grid (main + sidebar)
- `.dashboard-main-column` - Left column content
- `.dashboard-sidebar` - Right sidebar (320px)

### Quick Actions
- `.quick-actions-section` - 2-column button grid
- `.quick-action-btn` - Base button styling
- `.quick-action-btn-primary` - Green button
- `.quick-action-btn-info` - Blue button
- `.quick-action-icon-wrapper` - 48×48 icon container
- `.quick-action-text` - Title + subtitle wrapper
- `.quick-action-title` - Title text (16px)
- `.quick-action-subtitle` - Subtitle text (12px)
- `.quick-action-arrow` - Arrow indicator

### Overview Grid
- `.overview-section` - Grid section wrapper
- `.section-heading` - Section title with icon
- `.overview-grid` - 6-column card grid
- `.overview-card` - Base card styling
- `.overview-card-success` - Green card
- `.overview-card-warning` - Orange card
- `.overview-card-destructive` - Red card
- `.overview-card-default` - Neutral card
- `.overview-card-draft` - Purple card
- `.overview-card-active` - Highlighted/selected state
- `.overview-card-icon` - Icon container (36×36)
- `.overview-icon-success` - Green icon
- `.overview-icon-warning` - Orange icon
- `.overview-icon-destructive` - Red icon
- `.overview-icon-default` - Neutral icon
- `.overview-card-content` - Text wrapper
- `.overview-card-label` - Small label (12px)
- `.overview-card-value` - Large number (32px)
- `.overview-card-sublabel` - Smaller text
- `.overview-badge-*` - Badge styling variants

### Alerts
- `.alert-card` - Base alert styling
- `.alert-card-destructive` - Red alert
- `.alert-card-warning` - Orange alert
- `.alert-card-header` - Alert header layout
- `.alert-title` - Alert title text
- `.alert-description` - Alert description text
- `.alert-content` - Content wrapper
- `.alert-preview-list` - Item list in alert
- `.alert-preview-item` - Individual preview item

### Panels
- `.collapsible-panel` - Panel wrapper
- `.panel-header` - Panel title area
- `.panel-title` - Title with icon
- `.panel-badge` - Badge in header
- `.panel-content` - Panel content area

### Notifications
- `.notification-card` - Individual notification
- `.notification-icon` - Icon wrapper (40×40)
- `.notification-icon[data-priority]` - Priority-colored icons
- `.notification-body` - Content wrapper
- `.notification-title` - Title text
- `.notification-description` - Description text
- `.notification-meta` - Metadata (patient, date, etc.)
- `.notification-actions` - Action buttons
- `.btn-action` - Action button
- `.btn-dismiss` - Dismiss button

### Appointments
- `.appointment-card` - Appointment item
- `.appointment-header` - Header layout
- `.appointment-patient` - Patient name
- `.appointment-time` - Time display
- `.appointment-meta` - Additional info

### Sidebar Widgets
- `.sidebar-card` - Widget container
- `.card-header` - Widget title
- `.card-body` - Widget content
- `.sidebar-item` - Item in widget
- `.sidebar-item-header` - Item header
- `.sidebar-item-title` - Item name
- `.sidebar-item-type` - Item type label
- `.btn-icon` - Icon button (24×24)
- `.btn-text` - Text link button

### Activity
- `.activity-item` - Activity timeline item
- `.activity-dot` - Colored dot
- `.activity-success` - Green activity
- `.activity-info` - Blue activity
- `.activity-warning` - Orange activity

### Intakes
- `.intake-item` - Intake form item
- `.intake-patient` - Patient name
- `.intake-progress` - Progress bar wrapper
- `.progress-bar` - Progress container
- `.progress-fill` - Filled portion

### POCs
- `.poc-item` - POC card
- `.poc-stats` - 4-column stats grid
- `.stat` - Individual stat
- `.util-bar` - Utilization bar

### Modals
- `.modal-overlay` - Semi-transparent background
- `.modal-content` - Modal box
- `.modal-large` - Large variant
- `.modal-header` - Header section
- `.modal-body` - Content section
- `.modal-actions` - Footer buttons
- `.form-group` - Form field wrapper
- `.form-input` - Input field

### Buttons
- `.btn-primary` - Green button
- `.btn-secondary` - Outlined button
- `.btn-text` - Text-only link
- `.btn-icon` - Icon-only button
- `.btn-retry` - Retry action
- `.btn-sign` - Sign action
- `.btn-close` - Close/dismiss button

### Badges
- `.badge` - Base badge
- `.badge-success` - Green badge
- `.badge-warning` - Orange badge
- `.badge-destructive` - Red badge
- `.badge-info` - Blue badge
- `.badge-primary` - Green variant

## Responsive Design Classes

### Breakpoints
- **Desktop**: >1200px (grid-template-columns: 1fr 320px)
- **Tablet**: 768px-1200px (grid-template-columns: 1fr, sidebar hidden)
- **Mobile**: <768px (single column layout)

### Key Responsive Changes
- Overview grid: 6 cols → 3 cols → 2 cols → 1 col
- Quick actions: 2 cols → 1 col
- Sidebar: visible → hidden on smaller screens
- Modal size: 500px → 90% on mobile
- Font sizes: Reduced on mobile for readability
- Padding: Reduced (24px → 16px → 16px) on smaller screens

## Color Token Reference

### Semantic Colors
| Name | Value | Use Case |
|------|-------|----------|
| Success (Primary) | #22c55e | Positive actions, primary buttons |
| Info (Blue) | #60a5fa | Secondary actions, info alerts |
| Warning (Orange) | #fbbf24 | Warnings, incomplete items |
| Destructive (Red) | #ef4444 | Errors, unsigned notes, deletes |
| Draft (Purple) | #a855f7 | Draft status, unsaved items |

### Background Colors
| Name | Value | Use Case |
|------|-------|----------|
| Dark BG | #1a1f28 | Page background |
| Card BG | #2e2e2e | Modal backgrounds |
| Dark Surface | Rgba opacity 2-5% | Hover states |

### Text Colors
| Name | Value | Use Case |
|------|-------|----------|
| Text Primary | #e5e5e5 | Main text |
| Text Secondary | #a3a3a3 | Labels, metadata |

## Animation & Interaction

### Transitions
- Default duration: 0.2s ease
- Modal entrance: 0.2s ease (slideUp)
- Hover states: 0.2s ease (background shift)

### Keyframes
- `fadeIn` - Overlay fade (modal entrance)
- `slideUp` - Modal content slide (entrance)

## Accessibility Features

### ARIA Attributes
- `role="alert"` - Error banners
- `role="button"` - Clickable cards
- `aria-label="Notifications"` - Icon-only buttons

### Keyboard Navigation
- `tabindex="0"` - Cards and action items
- Tab order follows logical flow
- Focus states visible on all interactive elements

### Semantic HTML
- `<header>` - Page header
- `<section>` - Major content sections
- `<button>` - All clickable actions
- `<input>` - Form fields
- `<label>` - Form labels

---

This architecture provides a complete, maintainable, and scalable component system fully aligned with the Figma design specification.
