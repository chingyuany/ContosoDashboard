# Data Model: Document Upload and Management

## Entities

### Document

- `DocumentId` (int, PK)
- `Title` (string, required)
- `Description` (string, optional)
- `Category` (string, required; one of: Project Documents, Team Resources, Personal Files, Reports, Presentations, Other)
- `FileType` (string, MIME type)
- `FilePath` (string, secure GUID-based stored filename)
- `FileSize` (long, bytes)
- `UploadDate` (DateTime)
- `UploadedByUserId` (int, FK to User)
- `AssociatedProjectId` (int?, nullable FK to Project)
- `UploadedByUser` (navigation property)
- `AssociatedProject` (navigation property)
- `Tags` (collection of `DocumentTag`)
- `Shares` (collection of `DocumentShare`)
- `Activities` (collection of `DocumentActivity`)
- `TaskAttachments` (collection of `TaskDocument`)

### DocumentTag

- `DocumentTagId` (int, PK)
- `DocumentId` (int, FK to Document)
- `Value` (string, normalized tag text)
- `Document` (navigation property)

### DocumentShare

- `DocumentShareId` (int, PK)
- `DocumentId` (int, FK to Document)
- `SharedWithUserId` (int, FK to User)
- `GrantedByUserId` (int, FK to User)
- `SharedDate` (DateTime)
- `Notes` (string, optional)
- `Document` (navigation property)
- `SharedWithUser` (navigation property)
- `GrantedByUser` (navigation property)

### DocumentActivity

- `ActivityId` (int, PK)
- `DocumentId` (int, FK to Document)
- `ActivityType` (string: Upload, Download, Delete, Share, UnShare, Edit)
- `UserId` (int, FK to User)
- `ActivityDate` (DateTime)
- `Details` (string, serialized activity details)
- `Document` (navigation property)
- `User` (navigation property)

### TaskDocument

- `TaskDocumentId` (int, PK)
- `TaskId` (int, FK to TaskItem)
- `DocumentId` (int, FK to Document)
- `AssociatedDate` (DateTime)
- `Task` (navigation property)
- `Document` (navigation property)

## Relationships

- `Document` is owned by one user and optionally associated with one project.
- `DocumentTag` enables many tags per document while keeping tags queryable.
- `DocumentShare` grants explicit access to additional users beyond project membership.
- `DocumentActivity` records audit events for uploads, downloads, deletes, shares, edits, and permission actions.
- `TaskDocument` attaches documents to tasks without duplicating document metadata.

## Validation and Indexing

- Index `Title`, `UploadDate`, `Category`, and `AssociatedProjectId` for browsing and sorting.
- Index `DocumentTag.Value` for search performance.
- Index `DocumentShare.SharedWithUserId` for shared-document lookups.
- Enforce required metadata on upload: `Title`, `Category`, `FilePath`, `FileSize`, `FileType`, `UploadDate`, `UploadedByUserId`.

## Notes on existing project models

- Use the existing `Project`, `User`, and `TaskItem` entities from the app.
- Reuse existing `Notification` infrastructure for in-app notifications rather than creating a separate notification store in this feature.
