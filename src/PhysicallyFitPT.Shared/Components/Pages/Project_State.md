# PROJECT_STATE.md

**Physically Fit PT (PFPT)**
*Last updated: 2026-01-21*

---

## ✅ CURRENT STATUS — STABLE & LOCKED

### Phase 1 + Scheduling Extension COMPLETE

All items below are **implemented, verified, and must not be modified** unless explicitly approved.

---

### 🔐 Authentication

* **Login.razor**
* PIN-only authentication
* Uses `IUserService`
* Clean build confirmed

---

### 🏠 Dashboard

* **Dashboard.razor**
* Read-only metrics
* Uses `IDataService`
* Clean build confirmed

---

### 👥 Patient Management

#### Patient List

* **Patients.razor**
* Search + navigation
* Componentized:

  * PatientSearchBar
  * PatientListItem
* Routes:

  * `/patients`
  * `/patients/{patientId}`
* Clean build confirmed

#### Patient Profile (Read-Only)

* **PatientProfile.razor**
* Route: `/patients/{patientId:guid}`
* Summary tab only
* Uses `IDataService.GetPatientByIdAsync`
* Clean build confirmed

---

### 📅 Appointment Scheduling (Patient-Scoped)

* **Appointments.razor**
* Patient-scoped loading
* Schedule + cancel via `IDataService`
* No global “today” clinic view
* Clean build confirmed

---

## 🧱 ARCHITECTURAL RULES

* UI injects **IDataService only**
* Infrastructure services are wrapped internally
* Offline-first MAUI + Web parity required
* Incremental change sets only
* Clean build required after every change
* Locked areas must not be revisited

---

## 🚧 NEXT PLANNED WORK (Not Started)

* Change Set 6: Patient Profile Tabs (Read-Only)

  * Notes
  * Goals
  * Outcomes
* OR
* Phase 2: Clinical Documentation (SOAP Viewer)

*No code started. Approval required.*

---

## 🛑 STOP CONDITIONS

* Build failure
* Scope creep
* Service contract changes
* Modification of locked components

When in doubt: **STOP and request approval.**
