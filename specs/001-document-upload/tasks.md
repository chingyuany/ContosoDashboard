# Tasks: Document Upload and Management

**Input**: Design documents from `/specs/001-document-upload/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Initialize the document management feature in the existing ContosoDashboard app.

- [x] T001 [P] Add upload storage configuration to `ContosoDashboard/appsettings.json` and `ContosoDashboard/appsettings.Development.json`
- [x] T002 [P] Add new navigation entry for Documents in `ContosoDashboard/Shared/NavMenu.razor`
- [ ] T003 [P] Create `ContosoDashboard/Pages/Documents.razor` as the entry page for document upload, search, and lists
- [x] T004 [P] Add `ContosoDashboard/Services/IFileStorageService.cs` to define file storage operations
- [x] T005 [P] Create `ContosoDashboard/Services/LocalFileStorageService.cs` to store uploaded files outside `wwwroot`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Implement core models, storage abstractions, and authorization infrastructure required by all document stories.

- [x] T006 Add document-related entity models in `ContosoDashboard/Models/Document.cs`, `ContosoDashboard/Models/DocumentTag.cs`, `ContosoDashboard/Models/DocumentShare.cs`, `ContosoDashboard/Models/DocumentActivity.cs`, and `ContosoDashboard/Models/TaskDocument.cs`
- [x] T007 Update `ContosoDashboard/Data/ApplicationDbContext.cs` to register new document entity DbSet properties and configure relationships
- [x] T008 Create `ContosoDashboard/Services/DocumentService.cs` to encapsulate upload, search, authorization, sharing, attachment, and audit logic
- [x] T009 Update `ContosoDashboard/Program.cs` to register `DocumentService`, `IFileStorageService`, and document authorization policies
- [ ] T010 Update notification registration in `ContosoDashboard/Services/NotificationService.cs` or `Program.cs` so document share actions can trigger in-app notifications
- [x] T011 Add `ContosoDashboard/Pages/DocumentDetails.razor` and supporting code to provide a reusable view for document preview, metadata editing, and sharing

---

## Phase 3: User Story 1 - Upload and Store Documents (Priority: P1)

**Goal**: Enable authenticated users to upload documents with required metadata, store files securely, and validate file type/size.

**Independent Test**: Upload a PDF with title/category/project metadata, verify the file is stored under the configured upload folder, metadata persists in the database, and unauthorized project uploads are rejected.

- [x] T012 [US1] Add upload metadata model support in `ContosoDashboard/Models/DocumentUploadModel.cs`
- [ ] T013 [US1] Implement file size and MIME type validation in `ContosoDashboard/Services/DocumentService.cs`
- [x] T014 [US1] Implement document upload flow in `ContosoDashboard/Pages/Documents.razor`
- [x] T015 [US1] Implement secure file save in `ContosoDashboard/Services/LocalFileStorageService.cs` using GUID-based stored filenames outside `wwwroot`
- [ ] T016 [US1] Add project membership authorization checks in `ContosoDashboard/Services/DocumentService.cs` for uploads to associated projects
- [ ] T017 [US1] Add audit logging for upload actions to `DocumentActivity` in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T018 [US1] Add user-facing error messages for "File exceeds maximum size of 25 MB" and unsupported file types in `ContosoDashboard/Pages/Documents.razor`

---

## Phase 4: User Story 2 - View and Search Personal Documents (Priority: P1)

**Goal**: Provide users with a searchable, sortable My Documents view and project document listing.

**Independent Test**: Upload multiple documents, verify they appear in My Documents with correct metadata, search by title/tags/project, and filter by project.

- [ ] T019 [US2] Implement My Documents query in `ContosoDashboard/Services/DocumentService.cs` filtered to the current user
- [ ] T020 [US2] Add search and filter UI controls in `ContosoDashboard/Pages/Documents.razor`
- [ ] T021 [US2] Implement document search by title, description, tags, uploader name, and project in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T022 [US2] Add sort-by-upload-date and project filter support in `ContosoDashboard/Pages/Documents.razor`
- [ ] T023 [US2] Add `No documents found` empty state messaging and suggested actions to `ContosoDashboard/Pages/Documents.razor`
- [ ] T024 [US2] Display project-specific documents in `ContosoDashboard/Pages/ProjectDetails.razor` for the current user's project membership

---

## Phase 5: User Story 3 - Access Control and IDOR Protection (Priority: P1)

**Goal**: Secure document downloads, preview, metadata edits, and deletes with service-layer authorization and IDOR protection.

**Independent Test**: Confirm a project member can download a document, a non-member receives 403, and unauthorized direct access is logged.

- [ ] T025 [US3] Implement `GetDocumentDetailsAsync` and `DownloadDocumentAsync` in `ContosoDashboard/Services/DocumentService.cs` with service-layer authorization
- [ ] T026 [US3] Add download and preview links in `ContosoDashboard/Pages/DocumentDetails.razor`
- [ ] T027 [US3] Implement PDF/image preview support in `ContosoDashboard/Pages/DocumentDetails.razor`
- [ ] T028 [US3] Implement authorization failure logging in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T029 [US3] Add metadata edit and replace-file support in `ContosoDashboard/Pages/DocumentDetails.razor`
- [ ] T030 [US3] Add delete confirmation and document removal support in `ContosoDashboard/Pages/DocumentDetails.razor`
- [ ] T031 [US3] Ensure delete actions also remove the stored file via `ContosoDashboard/Services/LocalFileStorageService.cs` and record a `DocumentActivity`

---

## Phase 6: User Story 4 - Share Documents and Receive Notifications (Priority: P2)

**Goal**: Let document owners share files with specific users and notify recipients in-app.

**Independent Test**: Share a document with another user, confirm it appears in Shared with Me, and verify the recipient receives a notification and can download it.

- [ ] T032 [US4] Implement `ShareDocumentAsync` and `GetSharedWithMeDocumentsAsync` in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T033 [US4] Add document sharing UI with recipient selection in `ContosoDashboard/Pages/DocumentDetails.razor`
- [ ] T034 [US4] Add a `Shared with Me` section in `ContosoDashboard/Pages/Documents.razor`
- [ ] T035 [US4] Create in-app notification events for shared documents in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T036 [US4] Update authorization logic so explicitly shared recipients can download shared documents even if they are not project members

---

## Phase 7: User Story 5 - Attach Documents to Tasks (Priority: P2)

**Goal**: Allow attaching existing documents or uploading new documents from task details so task-related files remain organized.

**Independent Test**: Attach a document to a task, verify it appears on the task detail page, and confirm the document is associated with the task's project.

- [ ] T037 [US5] Implement `AttachDocumentToTaskAsync` in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T038 [US5] Create `ContosoDashboard/Pages/TaskDetails.razor` to support task-level document attachments and attached document display
- [ ] T039 [US5] Add UI support in `ContosoDashboard/Pages/Tasks.razor` or `ContosoDashboard/Pages/TaskDetails.razor` for attaching existing documents to a task
- [ ] T040 [US5] Ensure attached documents inherit the task project association in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T041 [US5] Display attached documents with download links in `ContosoDashboard/Pages/TaskDetails.razor`

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Finish UX, security hardening, documentation, and feature-wide cleanup.

- [ ] T042 [P] Add Recent Documents widget to `ContosoDashboard/Pages/Index.razor`
- [ ] T043 [P] Add document metadata editing, category/tag update, and replace-file validation in `ContosoDashboard/Pages/DocumentDetails.razor`
- [ ] T044 [P] Add or update EF Core migration files for the new document entities in `ContosoDashboard/Data/` or `ContosoDashboard/Migrations/`
- [ ] T045 [P] Add audit logging for download, delete, share, and edit actions in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T046 [P] Review and update feature documentation in `specs/001-document-upload/quickstart.md` and `specs/001-document-upload/research.md`
- [ ] T047 [P] Validate the Documents page empty-state and error messaging for search, upload, and authorization failures in `ContosoDashboard/Pages/Documents.razor`

---

## Dependencies & Execution Order

### Phase Dependencies

- Phase 1 Setup: can begin immediately.
- Phase 2 Foundational: depends on Phase 1 completion and blocks user story implementation.
- Phase 3+ User Stories: depend on Foundational completion; each story should be independently testable after that.
- Final Polish: depends on all desired user stories being implemented.

### User Story Dependencies

- User Story 1 (P1): can start after Foundational and provides the MVP upload capability.
- User Story 2 (P1): can start after Foundational and is independent from other stories.
- User Story 3 (P1): can start after Foundational and establishes security/IDOR protection.
- User Story 4 (P2): relies on sharing infrastructure but should remain independently testable once Foundational is complete.
- User Story 5 (P2): depends on document and task attachment infrastructure but should be independently testable after Foundational.

### Parallel Opportunities

- Setup tasks T001–T005 are parallelizable across configuration, navigation, page scaffolding, and storage service creation.
- Foundational tasks T006–T011 can be executed in parallel where they touch different files.
- Story-specific model/service/UI tasks within separate user stories can proceed in parallel once the foundation is ready.
- Polish tasks T042–T047 are parallelizable across documentation, UI cleanup, logging, and migration work.

## Implementation Strategy

### MVP First

1. Complete Phase 1 and Phase 2 and Phase 3.
2. Implement User Story 1 as the MVP upload experience.
3. Validate User Story 1 independently before adding search or security enhancements.

### Incremental Delivery

1. Deliver upload and secure storage (US1).
2. Add personal search and project listing (US2).
3. Harden access control and preview/download security (US3).
4. Add sharing and notifications (US4).
5. Add task attachments (US5).
6. Finish with polish, docs, and migrations.

### Team Strategy

- One developer can complete Foundation and User Story 1 first.
- Another developer can parallelize User Story 2 or User Story 3 after Foundation is complete.
- User Story 4 and User Story 5 can follow as separate enhancements once the base document flow is stable.
