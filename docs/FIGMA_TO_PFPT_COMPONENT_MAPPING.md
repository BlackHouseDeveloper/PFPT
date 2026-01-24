# Figma Prototype → PFPT Blazor Component Mapping

**Project**: PFPT (Physically Fit PT)  
**Source**: PTDoc Prototype v5 (Figma Make)  
**Target**: .NET 8 MAUI Blazor + Razor Class Library  
**Date**: January 21, 2026

---

## Mapping Principles

- **Shared UI**: Components in `PhysicallyFitPT.Shared/Components/` (Razor Class Library)
- **MAUI Pages**: Top-level pages in `PhysicallyFitPT.Maui/Pages/`
- **Web Pages**: Top-level pages in `PhysicallyFitPT.Web/Pages/`
- **Services**: Infrastructure layer services (already defined in `PhysicallyFitPT.Infrastructure/Services/`)
- **Domain**: Core models in `PhysicallyFitPT.Core/`

---

## 1. Authentication & Security

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `LoginForm.tsx` | `Login.razor` | Shared/Pages | `IAuthService` | PIN-based auth; offline-first; stores CurrentUserId |
| `ForgotPinModal.tsx` | `ForgotPin.razor` | Shared/Components/Auth | `IAuthService` | Recovery flow; may require admin override |
| `PinChangeForm.tsx` | `ChangePin.razor` | Shared/Components/Auth | `IAuthService` | Settings sub-component |
| `TwoFactorSetup.tsx` | `TwoFactorSetup.razor` | Shared/Components/Auth | `IAuthService`, `ITwoFactorService` | Future enhancement; stub for now |
| `TwoFactorVerification.tsx` | `TwoFactorVerification.razor` | Shared/Components/Auth | `ITwoFactorService` | Future enhancement |
| `SignUpFormEnhanced.tsx` | `Register.razor` | Shared/Pages | `IAuthService`, `IUserService` | Clinician registration; admin approval workflow |

**State**: Authentication state managed via `IAuthService.CurrentUserId` (in-memory session)  
**Offline**: Local PIN validation against SQLite user records  
**Security**: No PHI logged; audit all login attempts

---

## 2. Dashboard & Home

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `DashboardEnhanced.tsx` | `Dashboard.razor` | Shared/Pages | `IPatientService`, `IAppointmentService`, `INoteService` | Aggregate stats; quick actions |
| `DashboardPOCWidget.tsx` | `PocWidget.razor` | Shared/Components/Dashboard | `IPocService` | Plan of Care tracking widget |
| `IncompleteIntakesWidget.tsx` | `IncompleteIntakesWidget.razor` | Shared/Components/Dashboard | `IIntakeService` | Alerts for pending intakes |
| `WorkflowDashboard.tsx` | `WorkflowDashboard.razor` | Shared/Components/Dashboard | Multiple services | Admin/practice management overview |

**State**: Dashboard aggregates data from multiple services; refresh on navigation  
**Offline**: Display cached counts; show sync status indicator  
**Performance**: Use async loading for widgets; skeleton loaders

---

## 3. Patient Management

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `PatientsListPage.tsx` | `Patients.razor` | Shared/Pages | `IPatientService` | List, search, filter patients |
| `PatientProfile.tsx` | `PatientProfile.razor` | Shared/Pages | `IPatientService`, `IAppointmentService`, `INoteService` | Patient detail view; tabs for appointments, notes, documents |
| `PatientProfileEnhanced.tsx` | _Use base `PatientProfile.razor`_ | Shared/Pages | Multiple services | Enhanced version merges into base component |
| `PatientIntakeForm.tsx` | `IntakeForm.razor` | Shared/Pages | `IIntakeService`, `IPatientService` | Multi-step intake wizard |
| `PatientIntakeFormEnhanced.tsx` | _Use base `IntakeForm.razor`_ | Shared/Pages | Multiple services | Enhanced features merge into base |
| `PatientIntakeReview.tsx` | `IntakeReview.razor` | Shared/Components/Intake | `IIntakeService` | Review/approval step |
| `PatientPayerInformation.tsx` | `PayerInformation.razor` | Shared/Components/Patient | `IInsuranceService` | Insurance/billing info |
| `PatientCheckInKiosk.tsx` | `CheckInKiosk.razor` | Shared/Pages | `IAppointmentService`, `ICheckInService` | Patient-facing check-in |
| `PatientFeedbackSystem.tsx` | `FeedbackSystem.razor` | Shared/Components/Patient | `IFeedbackService` | Post-visit surveys |

**State**: Patient context flows via route parameter (patientId); services fetch related data  
**Offline**: Full CRUD on patients; sync conflicts resolved via timestamp + user choice  
**HIPAA**: Ensure no PHI in client-side logs or browser storage

---

## 4. Appointments & Scheduling

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `AppointmentsPage.tsx` | `Appointments.razor` | Shared/Pages | `IAppointmentService`, `IPatientService` | Main scheduling interface |
| `SchedulePage.tsx` | `Schedule.razor` | Shared/Pages | `IAppointmentService`, `IScheduleService` | Clinician schedule view |
| `ScheduleManager.tsx` | `ScheduleManager.razor` | Shared/Components/Schedule | `IScheduleService` | Admin schedule management |
| `AppointmentsWeekView.tsx` | `WeekView.razor` | Shared/Components/Schedule | `IAppointmentService` | Week calendar view |
| `AppointmentListView.tsx` | `AppointmentList.razor` | Shared/Components/Schedule | `IAppointmentService` | List view of appointments |
| `AppointmentTimelineView.tsx` | `TimelineView.razor` | Shared/Components/Schedule | `IAppointmentService` | Timeline/day view |
| `AppointmentEditModal.tsx` | `AppointmentEditDialog.razor` | Shared/Components/Schedule | `IAppointmentService`, `IPatientService` | Create/edit appointment |
| `AppointmentFilterToolbar.tsx` | `AppointmentFilter.razor` | Shared/Components/Schedule | None (UI state) | Filter/sort toolbar |
| `CalendarWeekView.tsx` | `CalendarWeek.razor` | Shared/Components/Schedule | `IAppointmentService` | Reusable week calendar |
| `scheduling/WeekView.tsx` | _Use `WeekView.razor`_ | Shared/Components/Schedule | `IAppointmentService` | Consolidate week views |
| `scheduling/DayView.tsx` | `DayView.razor` | Shared/Components/Schedule | `IAppointmentService` | Single-day view |
| `scheduling/AppointmentDialog.tsx` | _Use `AppointmentEditDialog.razor`_ | Shared/Components/Schedule | Multiple services | Consolidate dialogs |
| `scheduling/ScheduleBlockDialog.tsx` | `ScheduleBlockDialog.razor` | Shared/Components/Schedule | `IScheduleService` | Block out time slots |
| `scheduling/VisitTypeSettings.tsx` | `VisitTypeSettings.razor` | Shared/Components/Settings | `IVisitTypeService` | Configure visit types |
| `VisitTypeConfigurator.tsx` | _Use `VisitTypeSettings.razor`_ | Shared/Components/Settings | `IVisitTypeService` | Consolidate configurators |
| `AuthorizationCalendar.tsx` | `AuthorizationCalendar.razor` | Shared/Components/Schedule | `IAuthorizationService` | Track auth visits remaining |

**State**: Appointment state managed via `IAppointmentService`; local optimistic updates  
**Offline**: Full offline scheduling; sync conflicts require user resolution (timestamp-based)  
**Performance**: Virtualize large calendar views; lazy-load appointment details

---

## 5. Clinical Documentation (SOAP Notes)

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `SOAPNoteBuilder.tsx` | `SoapNoteBuilder.razor` | Shared/Pages | `INoteService`, `IPatientService`, `IGoalService` | Primary note-taking interface |
| `BlankSOAPNote.tsx` | _Template in `SoapNoteBuilder.razor`_ | Shared/Pages | `INoteService` | Starting template |
| `NotesPage.tsx` | `Notes.razor` | Shared/Pages | `INoteService`, `IPatientService` | List/search patient notes |
| `DischargeNoteBuilder.tsx` | `DischargeNoteBuilder.razor` | Shared/Pages | `INoteService`, `IDischargeService` | Discharge documentation |
| `DryNeedlingNote.tsx` | `DryNeedlingNote.razor` | Shared/Pages | `INoteService`, `IDryNeedlingService` | Specialized note type |
| `ProgressNoteComparison.tsx` | `ProgressComparison.razor` | Shared/Components/Notes | `INoteService`, `IOutcomeMeasureService` | Compare note to previous |
| `NoteReviewSubmission.tsx` | `NoteReview.razor` | Shared/Components/Notes | `INoteService`, `IValidationService` | Pre-sign review step |
| `NoteSigningModal.tsx` | `NoteSigningDialog.razor` | Shared/Components/Notes | `INoteService`, `IAuthService` | Digital signature/finalize |
| `NoteVersionHistory.tsx` | `NoteVersionHistory.razor` | Shared/Components/Notes | `INoteService`, `IAuditService` | View note amendments/history |
| `AmendNoteDialog.tsx` | `AmendNoteDialog.razor` | Shared/Components/Notes | `INoteService` | Amendment workflow |
| `NoteSignOffValidator.tsx` | `SignOffValidator.razor` | Shared/Components/Notes | `IValidationService` | Pre-sign validation checks |
| `CosignWorkflow.tsx` | `CosignWorkflow.razor` | Shared/Components/Notes | `INoteService`, `IAuthService` | Supervisor co-signature |

### SOAP Section Components

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `ChiefComplaintSelector.tsx` | `ChiefComplaintSelector.razor` | Shared/Components/Soap | `IClinicalDataService` | Subjective: chief complaint |
| `EvalSubjectiveQuestions.tsx` | `EvalSubjectiveQuestions.razor` | Shared/Components/Soap | `IClinicalDataService` | Eval-specific subjective prompts |
| `ProgressSubjectiveQuestions.tsx` | `ProgressSubjectiveQuestions.razor` | Shared/Components/Soap | `IClinicalDataService` | Progress note subjective |
| `PainDiagramForm.tsx` | `PainDiagram.razor` | Shared/Components/Soap | `IClinicalDataService` | Interactive body chart |
| `PainDiagramFormEnhanced.tsx` | _Use base `PainDiagram.razor`_ | Shared/Components/Soap | Multiple services | Enhanced features merge into base |
| `PainDescriptionSelector.tsx` | `PainDescriptionSelector.razor` | Shared/Components/Soap | `IClinicalDataService` | Pain characteristics |
| `ClinicalObservationsSelector.tsx` | `ClinicalObservationsSelector.razor` | Shared/Components/Soap | `IClinicalDataService` | Objective: observations |
| `PosturalAssessmentSelector.tsx` | `PosturalAssessmentSelector.razor` | Shared/Components/Soap | `IClinicalDataService` | Objective: posture |
| `GaitAnalysisSelector.tsx` | `GaitAnalysisSelector.razor` | Shared/Components/Soap | `IClinicalDataService` | Objective: gait |
| `ROMInputGrid.tsx` | `RomInputGrid.razor` | Shared/Components/Soap | `IClinicalDataService` | Objective: range of motion |
| `MMTInputGrid.tsx` | `MmtInputGrid.razor` | Shared/Components/Soap | `IClinicalDataService` | Objective: manual muscle testing |
| `ROMMMTComparison.tsx` | `RomMmtComparison.razor` | Shared/Components/Soap | `IClinicalDataService`, `INoteService` | Compare to previous values |
| `SpecialTestsSelector.tsx` | `SpecialTestsSelector.razor` | Shared/Components/Soap | `IClinicalDataService` | Objective: special tests |
| `FunctionalScreenSection.tsx` | `FunctionalScreen.razor` | Shared/Components/Soap | `IClinicalDataService` | Objective: functional testing |
| `NeuroScreenSection.tsx` | `NeuroScreen.razor` | Shared/Components/Soap | `IClinicalDataService` | Objective: neuro assessment |
| `MeasurementsSelector.tsx` | `MeasurementsSelector.razor` | Shared/Components/Soap | `IClinicalDataService` | Objective: measurements (girth, etc.) |
| `PelvicFloorEvaluation.tsx` | `PelvicFloorEvaluation.razor` | Shared/Components/Soap | `IClinicalDataService` | Specialized evaluation |
| `DeficitsAssessment.tsx` | `DeficitsAssessment.razor` | Shared/Components/Soap | `IClinicalDataService` | Assessment: deficits summary |
| `FunctionalLimitationsSelector.tsx` | `FunctionalLimitationsSelector.razor` | Shared/Components/Soap | `IClinicalDataService` | Assessment: functional limits |
| `StructuredAssessments.tsx` | `StructuredAssessments.razor` | Shared/Components/Soap | `IClinicalDataService` | Assessment templates |
| `PlanSection.tsx` | `PlanSection.razor` | Shared/Components/Soap | `IGoalService`, `IInterventionService` | Plan: goals & interventions |
| `GoalsInterventionsEnhanced.tsx` | _Use `PlanSection.razor`_ | Shared/Components/Soap | Multiple services | Consolidate plan components |
| `SmartGoalGenerator.tsx` | `SmartGoalGenerator.razor` | Shared/Components/Soap | `IGoalService`, `IClinicalDataService` | AI-assisted goal generation |
| `FollowUpInstructionsSelector.tsx` | `FollowUpInstructionsSelector.razor` | Shared/Components/Soap | `IClinicalDataService` | Plan: follow-up instructions |
| `FollowUpScheduling.tsx` | `FollowUpScheduling.razor` | Shared/Components/Soap | `IAppointmentService` | Schedule next visit from note |

**State**: SOAP note state managed by `INoteService`; auto-save every N seconds (offline-first)  
**Offline**: Notes fully editable offline; signed notes lock and queue for sync  
**HIPAA**: Ensure signed notes are immutable (amendments create addenda records)  
**Carry-forward**: Previous note data loaded via `INoteService.GetLatestNoteForPatient()`

---

## 6. Outcome Measures & Goals

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `OutcomeMeasures.tsx` | `OutcomeMeasures.razor` | Shared/Components/Clinical | `IOutcomeMeasureService` | List and administer outcome measures |
| `OutcomeMeasureGoalsGenerator.tsx` | `OutcomeMeasureGoalsGenerator.razor` | Shared/Components/Clinical | `IOutcomeMeasureService`, `IGoalService` | Generate goals from outcome scores |
| `GoalStatusReview.tsx` | `GoalStatusReview.razor` | Shared/Components/Clinical | `IGoalService` | Review/update goal progress |
| `GoalsSidebar.tsx` | `GoalsSidebar.razor` | Shared/Components/Clinical | `IGoalService` | Persistent goals sidebar in note UI |
| `DischargeGoalsSummary.tsx` | `DischargeGoalsSummary.razor` | Shared/Components/Clinical | `IGoalService`, `IDischargeService` | Summarize goals at discharge |
| `DischargeOutcomesSummary.tsx` | `DischargeOutcomesSummary.razor` | Shared/Components/Clinical | `IOutcomeMeasureService`, `IDischargeService` | Summarize outcomes at discharge |

**State**: Goals and outcome measures persist via respective services; link to patient and POC  
**Offline**: Full offline CRUD for goals; outcome measure scoring offline-capable  
**Clinical Logic**: Goal progress calculations, outcome measure scoring algorithms

---

## 7. Exercises & Home Programs

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `ExerciseModule.tsx` | `ExerciseModule.razor` | Shared/Components/Exercise | `IExerciseService` | Search and select exercises |
| `ExerciseTableEnhanced.tsx` | `ExerciseTable.razor` | Shared/Components/Exercise | `IExerciseService` | Display selected exercises with parameters |
| `ExercisesTab.tsx` | `ExercisesTab.razor` | Shared/Components/Exercise | `IExerciseService` | Patient profile tab for HEP |
| `HEPExportButton.tsx` | `HepExportButton.razor` | Shared/Components/Exercise | `IExerciseService`, `IPdfExportService` | Export HEP to PDF |

**State**: Exercises managed via `IExerciseService`; link to patient and note  
**Offline**: Exercise library available offline; HEP PDF generation offline-capable  
**Export**: HEP PDF must match approved template; role-gated export

---

## 8. Billing & Coding

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `CPTCodesModule.tsx` | `CptCodesModule.razor` | Shared/Components/Billing | `IBillingService`, `ICptCodeService` | CPT code selection |
| `EnhancedCPTModule.tsx` | _Use base `CptCodesModule.razor`_ | Shared/Components/Billing | Multiple services | Consolidate CPT modules |
| `ICD10Search.tsx` | `Icd10Search.razor` | Shared/Components/Billing | `IIcd10Service` | ICD-10 code search/selection |
| `ClaimsManagement.tsx` | `ClaimsManagement.razor` | Shared/Pages | `IBillingService`, `IClaimsService` | Claims submission/tracking |
| `POCTracker.tsx` | `PocTracker.razor` | Shared/Components/Billing | `IPocService` | Plan of Care tracking |
| `POCDateCalculator.tsx` | `PocDateCalculator.razor` | Shared/Components/Billing | `IPocService` | Calculate POC dates/visits |
| `PriorAuthorizationTracker.tsx` | `PriorAuthTracker.razor` | Shared/Components/Billing | `IAuthorizationService` | Track prior auths |
| `InsuranceVerificationCheck.tsx` | `InsuranceVerificationCheck.razor` | Shared/Components/Billing | `IInsuranceService` | Verify eligibility/benefits |
| `InsuranceVerificationPanel.tsx` | `InsuranceVerificationPanel.razor` | Shared/Components/Billing | `IInsuranceService` | Display verification results |

**State**: Billing data managed via respective services; link to appointments and notes  
**Offline**: CPT/ICD codes available offline; claims queue for submission when online  
**Compliance**: Audit all billing actions; validate code combinations

---

## 9. Settings & Configuration

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `SettingsPage.tsx` | `Settings.razor` | Shared/Pages | Multiple services | Main settings hub |
| `SettingsCenter.tsx` | _Use base `Settings.razor`_ | Shared/Pages | Multiple services | Consolidate settings pages |
| `AccessibilitySettings.tsx` | `AccessibilitySettings.razor` | Shared/Components/Settings | `IAccessibilityService` | Accessibility preferences |
| `NotificationPreferences.tsx` | `NotificationPreferences.razor` | Shared/Components/Settings | `INotificationService` | Notification settings |
| `AutoCheckInMessagingSettings.tsx` | `AutoCheckInMessagingSettings.razor` | Shared/Components/Settings | `ICheckInService` | Auto check-in config |
| `RoleManagementSystem.tsx` | `RoleManagement.razor` | Shared/Components/Settings | `IRoleService`, `IAuthService` | Admin: manage user roles |
| `PermissionEditor.tsx` | `PermissionEditor.razor` | Shared/Components/Settings | `IRoleService` | Admin: edit role permissions |
| `settings/RolesPermissionsSettings.tsx` | _Use `RoleManagement.razor`_ | Shared/Components/Settings | `IRoleService` | Consolidate role settings |
| `settings/PermissionMatrixEditor.tsx` | _Use `PermissionEditor.razor`_ | Shared/Components/Settings | `IRoleService` | Consolidate permission editors |
| `settings/DocumentationSettings.tsx` | `DocumentationSettings.razor` | Shared/Components/Settings | `ISettingsService` | Documentation preferences |
| `settings/BillingCodingSettings.tsx` | `BillingCodingSettings.razor` | Shared/Components/Settings | `IBillingService` | Billing/coding config |
| `settings/SchedulingSettings.tsx` | `SchedulingSettings.razor` | Shared/Components/Settings | `IScheduleService` | Scheduling config |
| `settings/ScheduleBlockEditor.tsx` | _Use `ScheduleBlockDialog.razor`_ | Shared/Components/Schedule | `IScheduleService` | Schedule block editor |
| `settings/VisitTypeEditor.tsx` | _Use `VisitTypeSettings.razor`_ | Shared/Components/Settings | `IVisitTypeService` | Visit type editor |
| `settings/SystemPreferencesSettings.tsx` | `SystemPreferences.razor` | Shared/Components/Settings | `ISettingsService` | System-level preferences |
| `settings/AIOutcomesSettings.tsx` | `AiOutcomesSettings.razor` | Shared/Components/Settings | `IAiService` | AI feature configuration |
| `settings/NotificationMessagingSettings.tsx` | _Use `NotificationPreferences.razor`_ | Shared/Components/Settings | `INotificationService` | Consolidate notification settings |
| `settings/PatientPortalSettings.tsx` | `PatientPortalSettings.razor` | Shared/Components/Settings | `IPortalService` | Patient portal config |

**State**: Settings persisted via `ISettingsService`; user-scoped and system-scoped  
**Offline**: Settings available offline; sync changes when online  
**Admin**: Role-based access to admin settings

---

## 10. Reports & Export

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `ReportsPage.tsx` | `Reports.razor` | Shared/Pages | `IReportService` | Report generation hub |
| `ExportCenterPage.tsx` | `ExportCenter.razor` | Shared/Pages | `IExportService`, `IPdfExportService` | Export/import data |
| `pdf/ExportDialog.tsx` | `ExportDialog.razor` | Shared/Components/Export | `IPdfExportService` | PDF export options dialog |
| `pdf/PDFTemplates.tsx` | _Templates in `IPdfExportService`_ | Infrastructure | `IPdfExportService` | PDF layout templates |
| `ProgressTrackingPage.tsx` | `ProgressTracking.razor` | Shared/Pages | `IReportService`, `IOutcomeMeasureService` | Patient progress reports |

**State**: Reports generated on-demand; export actions audited  
**Offline**: Queue exports for online processing if templates require cloud resources  
**HIPAA**: Role-gate exports; audit all PHI exports; match approved PDF template

---

## 11. Communication & Notifications

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `NotificationCenter.tsx` | `NotificationCenter.razor` | Shared/Components/Notifications | `INotificationService` | Global notification inbox |
| `NotificationCenterEnhanced.tsx` | _Use base `NotificationCenter.razor`_ | Shared/Components/Notifications | `INotificationService` | Consolidate notification centers |
| `CommunicationsTab.tsx` | `CommunicationsTab.razor` | Shared/Components/Patient | `INotificationService`, `IMessageService` | Patient communications log |
| `AutoCheckInMessagingSettings.tsx` | `AutoCheckInMessagingSettings.razor` | Shared/Components/Settings | `ICheckInService`, `IMessageService` | Auto check-in messaging |

**State**: Notifications managed via `INotificationService`; mark read/unread  
**Offline**: Queue outgoing messages; display unsynced status  
**HIPAA**: Secure messaging only; no PHI in push notifications

---

## 12. Workflow & Automation

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `workflow/NoteTypeSwitcher.tsx` | `NoteTypeSwitcher.razor` | Shared/Components/Workflow | `INoteService` | Switch between note types |
| `workflow/VisitTypeAutomation.tsx` | `VisitTypeAutomation.razor` | Shared/Components/Workflow | `IVisitTypeService`, `INoteService` | Auto-populate based on visit type |
| `workflow/POCValidator.tsx` | `PocValidator.razor` | Shared/Components/Workflow | `IPocService`, `IValidationService` | Validate POC compliance |
| `workflow/NonBillableNoteForm.tsx` | `NonBillableNoteForm.razor` | Shared/Components/Workflow | `INoteService` | Non-billable note template |
| `WorkflowAutomationDemo.tsx` | `WorkflowAutomation.razor` | Shared/Components/Workflow | Multiple services | Admin: configure automation rules |
| `IntakeStatusTracker.tsx` | `IntakeStatusTracker.razor` | Shared/Components/Workflow | `IIntakeService` | Track intake completion |
| `EndOfCareChecklist.tsx` | `EndOfCareChecklist.razor` | Shared/Components/Workflow | `IDischargeService`, `IValidationService` | Discharge checklist |
| `DischargePlanningSelector.tsx` | `DischargePlanningSelector.razor` | Shared/Components/Workflow | `IDischargeService` | Discharge planning options |
| `DischargeReasonSelector.tsx` | `DischargeReasonSelector.razor` | Shared/Components/Workflow | `IDischargeService` | Discharge reason dropdown |

**State**: Workflow state managed by respective services; trigger actions based on events  
**Offline**: Workflows execute offline; queue external actions (e.g., notifications) for sync  
**Validation**: Real-time validation as user progresses through workflows

---

## 13. Shared UI Components (shadcn/ui equivalents)

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `ui/button.tsx` | `Button.razor` | Shared/Components/UI | None | Reusable button component |
| `ui/card.tsx` | `Card.razor` | Shared/Components/UI | None | Card container component |
| `ui/dialog.tsx` | `Dialog.razor` | Shared/Components/UI | None | Modal dialog component |
| `ui/input.tsx` | `Input.razor` | Shared/Components/UI | None | Text input component |
| `ui/label.tsx` | `Label.razor` | Shared/Components/UI | None | Form label component |
| `ui/select.tsx` | `Select.razor` | Shared/Components/UI | None | Dropdown select component |
| `ui/textarea.tsx` | `TextArea.razor` | Shared/Components/UI | None | Multi-line text input |
| `ui/checkbox.tsx` | `Checkbox.razor` | Shared/Components/UI | None | Checkbox component |
| `ui/radio-group.tsx` | `RadioGroup.razor` | Shared/Components/UI | None | Radio button group |
| `ui/switch.tsx` | `Switch.razor` | Shared/Components/UI | None | Toggle switch component |
| `ui/tabs.tsx` | `Tabs.razor` | Shared/Components/UI | None | Tab navigation component |
| `ui/accordion.tsx` | `Accordion.razor` | Shared/Components/UI | None | Collapsible accordion |
| `ui/alert.tsx` | `Alert.razor` | Shared/Components/UI | None | Alert/notification banner |
| `ui/badge.tsx` | `Badge.razor` | Shared/Components/UI | None | Status badge component |
| `ui/calendar.tsx` | `Calendar.razor` | Shared/Components/UI | None | Date picker calendar |
| `ui/date-picker.tsx` | `DatePicker.razor` | Shared/Components/UI | None | Date picker input |
| `ui/table.tsx` | `Table.razor` | Shared/Components/UI | None | Data table component |
| `ui/toast.tsx` | `Toast.razor` | Shared/Components/UI | None | Toast notification |
| `ui/tooltip.tsx` | `Tooltip.razor` | Shared/Components/UI | None | Tooltip overlay |
| `ui/skeleton.tsx` | `Skeleton.razor` | Shared/Components/UI | None | Loading skeleton |
| `ui/progress.tsx` | `Progress.razor` | Shared/Components/UI | None | Progress bar |
| `ui/separator.tsx` | `Separator.razor` | Shared/Components/UI | None | Visual divider |
| `ui/avatar.tsx` | `Avatar.razor` | Shared/Components/UI | None | User avatar component |
| `ui/multi-select.tsx` | `MultiSelect.razor` | Shared/Components/UI | None | Multi-select dropdown |

**Notes**: These are pure UI components with no business logic; implement using Blazor component patterns; match Figma design tokens (colors, spacing, typography)

---

## 14. Layout & Navigation

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `AppShell.tsx` | `AppShell.razor` | Shared/Layouts | `IAuthService` | Main app shell/frame |
| `AppLayout.tsx` | _Use `AppShell.razor`_ | Shared/Layouts | Multiple services | Consolidate layouts |
| `GlobalHeader.tsx` | `Header.razor` | Shared/Layouts | `IAuthService`, `INotificationService` | Top navigation bar |
| `MobileHeader.tsx` | _Responsive `Header.razor`_ | Shared/Layouts | Multiple services | Mobile-optimized header |
| `Navigation.tsx` | `Navigation.razor` | Shared/Layouts | `IAuthService`, `IRoleService` | Main navigation menu |
| `PersistentNavigation.tsx` | _Use `Navigation.razor`_ | Shared/Layouts | Multiple services | Consolidate navigation components |
| `RoleBasedNav.tsx` | _Logic in `Navigation.razor`_ | Shared/Layouts | `IRoleService` | Role-based nav filtering |
| `StickySOAPNavigation.tsx` | `SoapNavigation.razor` | Shared/Components/Soap | None (UI state) | Sticky S-O-A-P section nav |
| `GlobalSearch.tsx` | `GlobalSearch.razor` | Shared/Layouts | `ISearchService` | Global search bar |
| `AdminQuickLinks.tsx` | `AdminQuickLinks.razor` | Shared/Layouts | `IAuthService` | Admin action shortcuts |

**State**: Layout/navigation state managed in-memory; persist user preferences (collapsed nav, etc.)  
**Offline**: Navigation fully functional offline; indicate online/offline status  
**Responsive**: Mobile-first design; adapt nav for mobile/tablet/desktop

---

## 15. Status Indicators & Feedback

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `OfflineStatusIndicator.tsx` | `OfflineStatusIndicator.razor` | Shared/Components/Status | `ISyncService` | Show offline/online status |
| `OfflineSyncStatus.tsx` | _Use `OfflineStatusIndicator.razor`_ | Shared/Components/Status | `ISyncService` | Consolidate status indicators |
| `AutoSaveIndicator.tsx` | `AutoSaveIndicator.razor` | Shared/Components/Status | `INoteService` | Show auto-save status |
| `AutoSyncIndicator.tsx` | `AutoSyncIndicator.razor` | Shared/Components/Status | `ISyncService` | Show sync progress |
| `SyncButton.tsx` | `SyncButton.razor` | Shared/Components/Status | `ISyncService` | Manual sync trigger |
| `SyncToast.tsx` | `SyncToast.razor` | Shared/Components/Status | `ISyncService` | Sync status toast |
| `ConflictBanner.tsx` | `ConflictBanner.razor` | Shared/Components/Status | `ISyncService` | Sync conflict alert |
| `ConflictResolutionBanner.tsx` | _Use `ConflictBanner.razor`_ | Shared/Components/Status | `ISyncService` | Consolidate conflict banners |
| `ConflictResolutionDialog.tsx` | `ConflictResolutionDialog.razor` | Shared/Components/Status | `ISyncService` | Resolve sync conflicts |
| `ComplianceBanner.tsx` | `ComplianceBanner.razor` | Shared/Components/Status | `IComplianceService` | HIPAA/compliance notices |

**State**: Status managed by respective services; UI components subscribe to service events  
**Offline**: Status indicators critical for offline-first UX; always visible  
**UX**: Non-intrusive status indicators; toast for transient messages

---

## 16. Clinical Reference & Library

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `ClinicalLibrary.tsx` | `ClinicalLibrary.razor` | Shared/Pages | `IClinicalReferenceService` | Browse clinical content library |
| `ClinicalLibraryPanel.tsx` | `ClinicalLibraryPanel.razor` | Shared/Components/Clinical | `IClinicalReferenceService` | Sidebar panel for quick reference |
| `RegionQuestionsForm.tsx` | `RegionQuestionsForm.razor` | Shared/Components/Clinical | `IClinicalReferenceService` | Region-specific questions |
| `FallRiskScreen.tsx` | `FallRiskScreen.razor` | Shared/Components/Clinical | `IClinicalReferenceService` | Fall risk assessment |
| `MotivationSupportAssessment.tsx` | `MotivationSupportAssessment.razor` | Shared/Components/Clinical | `IClinicalReferenceService` | Patient motivation/support assessment |
| `PreviousTreatmentSelector.tsx` | `PreviousTreatmentSelector.razor` | Shared/Components/Clinical | `IClinicalReferenceService` | Select previous treatments |
| `ConsentsForm.tsx` | `ConsentsForm.razor` | Shared/Components/Clinical | `IConsentService` | Patient consent forms |
| `DryNeedlingForm.tsx` | `DryNeedlingForm.razor` | Shared/Components/Clinical | `IDryNeedlingService` | Dry needling consent/documentation |
| `DryNeedlingTemplate.tsx` | _Template in `DryNeedlingForm.razor`_ | Shared/Components/Clinical | `IDryNeedlingService` | Consolidate dry needling components |

**State**: Clinical reference data cached locally; read-only content  
**Offline**: Full offline access to clinical reference library  
**Content**: Sync new/updated reference content during background sync

---

## 17. Specialty Components

| Figma Component | PFPT Component | Target Project | Service Dependencies | Notes |
|----------------|----------------|----------------|---------------------|-------|
| `AccessibleButton.tsx` | _Use base `Button.razor` with ARIA_ | Shared/Components/UI | None | Ensure all buttons are accessible |
| `EnhancedDatePicker.tsx` | _Use `DatePicker.razor` enhanced_ | Shared/Components/UI | None | Enhanced date picker with validation |
| `FilterToolbar.tsx` | `FilterToolbar.razor` | Shared/Components/UI | None (UI state) | Generic filter toolbar |
| `CollapsiblePanel.tsx` | `CollapsiblePanel.razor` | Shared/Components/UI | None | Collapsible container component |
| `ErrorBoundary.tsx` | `ErrorBoundary.razor` | Shared/Components/UI | `ILoggingService` | Global error boundary |
| `ThemeProvider.tsx` | `ThemeProvider.razor` | Shared/Components/UI | `IThemeService` | Light/dark theme provider |
| `ThemeToggle.tsx` | `ThemeToggle.razor` | Shared/Components/UI | `IThemeService` | Theme toggle button |
| `ContactsTab.tsx` | `ContactsTab.razor` | Shared/Components/Patient | `IContactService` | Patient contacts list |
| `SupportingDocumentsTab.tsx` | `DocumentsTab.razor` | Shared/Components/Patient | `IDocumentService` | Patient documents tab |

**Notes**: Specialty components provide specific UX patterns; ensure ARIA compliance

---

## 18. Utilities & Helpers (Non-Component)

| Figma File | PFPT Equivalent | Target Project | Notes |
|-----------|-----------------|----------------|-------|
| `utils/accessibility.ts` | `AccessibilityHelpers.cs` | Infrastructure/Utilities | ARIA helpers, keyboard nav |
| `utils/bodyPartData.ts` | `BodyPartData.cs` (exists) | Core | Body part/region data |
| `utils/clinicalData.ts` | `ClinicalDataHelpers.cs` | Infrastructure/Utilities | Clinical data lookups |
| `utils/dataStore.ts` | _EF Core + Services_ | Infrastructure | Data access handled by services |
| `utils/pdfExport.ts` | `PdfExportService.cs` | Infrastructure/Services | PDF generation using QuestPDF |
| `utils/pocCalculations.ts` | `PocCalculationService.cs` | Infrastructure/Services | POC date/visit calculations |
| `lib/accessibility.ts` | _Same as `utils/accessibility.ts`_ | Infrastructure/Utilities | Consolidate |
| `lib/api-client.ts` | `ApiClient.cs` | Infrastructure/Http | HTTP client wrapper |
| `lib/notifications.ts` | `NotificationService.cs` | Infrastructure/Services | Notification logic |
| `lib/roles.ts` | `RoleService.cs` | Infrastructure/Services | Role/permission logic |
| `lib/twoFactor.ts` | `TwoFactorService.cs` | Infrastructure/Services | 2FA logic (future) |

**Notes**: Convert TypeScript utilities to C# helper classes or services as appropriate

---

## 19. Context Providers (State Management)

| Figma Context | PFPT Equivalent | Target Project | Notes |
|--------------|-----------------|----------------|-------|
| `AccessibilityContext.tsx` | `AccessibilityState` (Cascading Parameter) | Shared/Services | Accessibility preferences |
| `AuthContext.tsx` | `IAuthService` (Scoped Service) | Infrastructure/Services | Authentication state |
| `NotificationContext.tsx` | `INotificationService` (Scoped Service) | Infrastructure/Services | Notification state |
| `PTDocDataContext.tsx` | _Multiple Services_ | Infrastructure/Services | Global data context split into services |

**Notes**: React Context → Blazor Cascading Parameters or Scoped Services (DI)

---

## Implementation Order (Recommended)

### Phase 1: Foundation (Weeks 1-2)
1. **UI Components** (`Shared/Components/UI/`) - Button, Input, Card, Dialog, etc.
2. **Layout** (`Shared/Layouts/`) - AppShell, Header, Navigation
3. **Authentication** - LoginForm, AuthService integration
4. **Dashboard** - Basic dashboard with stats

### Phase 2: Patient Management (Weeks 3-4)
5. **Patient List** - PatientsListPage, PatientService
6. **Patient Profile** - PatientProfile, detail views
7. **Patient Intake** - IntakeForm, multi-step wizard

### Phase 3: Scheduling (Weeks 5-6)
8. **Appointments** - AppointmentsPage, scheduling UI
9. **Calendar Views** - WeekView, DayView, TimelineView
10. **Appointment Management** - Create, edit, cancel workflows

### Phase 4: Clinical Documentation (Weeks 7-10)
11. **SOAP Note Builder** - Core note-taking interface
12. **SOAP Sections** - Subjective, Objective, Assessment, Plan components
13. **Clinical Input Grids** - ROM, MMT, pain diagrams
14. **Note Review & Signing** - Validation, signing, amendments

### Phase 5: Billing & Reports (Weeks 11-12)
15. **Billing Components** - CPT, ICD-10, POC tracking
16. **PDF Export** - Note export, HEP export
17. **Reports** - Progress reports, outcome tracking

### Phase 6: Settings & Admin (Week 13)
18. **Settings Pages** - User settings, system preferences
19. **Admin Tools** - Role management, permissions

### Phase 7: Polish & Optimization (Week 14)
20. **Offline Sync** - Conflict resolution, status indicators
21. **Accessibility** - ARIA, keyboard nav, high contrast
22. **Performance** - Virtualization, lazy loading, caching

---

## Key Architectural Decisions

### 1. Component Location Strategy
- **Pages**: Top-level routable components → `Shared/Pages/` or platform-specific
- **Components**: Reusable, composable components → `Shared/Components/{Domain}/`
- **UI Components**: Pure UI with no business logic → `Shared/Components/UI/`
- **Layouts**: Shell, header, nav → `Shared/Layouts/`

### 2. Service Layer
- All data access via service interfaces (defined in Infrastructure)
- Services injected via DI (registered in MauiProgram.cs / Program.cs)
- Offline-first: services handle local SQLite + background sync
- No direct EF Core usage in Razor components

### 3. State Management
- **Page-level state**: Component @code block
- **Shared state**: Scoped services (via DI)
- **Global state**: Singleton services (e.g., AuthService, SyncService)
- **Cross-component communication**: EventCallback parameters or service events

### 4. Offline-First Patterns
- Local SQLite as source of truth
- Optimistic UI updates
- Background sync service
- Conflict resolution dialogs when needed
- Sync status indicators always visible

### 5. HIPAA Compliance
- No PHI in client-side logs
- Role-based access control on all sensitive actions
- Audit logging for PHI access/modifications
- Signed notes immutable (amendments create addenda)
- PDF exports role-gated and audited

### 6. Naming Conventions
- Blazor pages: `{Name}.razor` (e.g., `Login.razor`)
- Blazor components: `{Name}.razor` (e.g., `PatientCard.razor`)
- Services: `I{Name}Service` (interface), `{Name}Service` (implementation)
- Models: PascalCase (e.g., `Patient`, `Appointment`)

---

## Next Steps

1. **Review & Approve**: Review this mapping document
2. **Confirm First Component**: Explicitly confirm which component to implement first (recommended: `Login.razor`)
3. **Incremental Implementation**: Implement one component/page at a time
4. **Build Verification**: Build after each file or tightly coupled set
5. **Maintain Clean Build**: Ensure solution compiles at every step
6. **Wait for Approval**: Wait for explicit approval before proceeding to next component

---

**End of Mapping Document**
