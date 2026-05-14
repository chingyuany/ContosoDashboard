# Research: Document Upload and Management

## Decision

Implement the feature within the existing ContosoDashboard Blazor Server app using local filesystem storage behind an abstraction layer. Store files outside the web root in `AppData/uploads` using GUID-based filenames, and keep metadata in SQL Server LocalDB via EF Core.

## Rationale

- Aligns with the project’s offline-first training goals and current constitution requirement for local-first storage.
- Preserves a cloud migration path by abstracting storage behind `IFileStorageService` so `LocalFileStorageService` can be replaced with Azure Blob storage later.
- Minimizes risk by keeping business logic in Services and enforcing authorization at the service layer, matching the Security by Design principle.
- Enables a straightforward implementation for MVP search and preview while leaving room for future enhancements.

## Alternatives considered

- Azure Blob Storage / cloud file service
  - Not chosen for MVP because the training project must run fully offline and local filesystem access is already available.
- Storing uploaded files in the web-accessible `wwwroot` directory
  - Rejected due to path traversal and direct access risks; secure storage outside the web root is mandatory.
- Using raw filenames instead of GUID-based storage keys
  - Rejected because GUID-based filenames prevent collisions and reduce the risk of leaking sensitive naming information.
- Implementing full-text search or search engine integration
  - Deferred until after MVP; simple EF Core string search over title, description, tags, uploader, and project is sufficient for 500 documents/user.
- Soft delete / versioning in MVP
  - Rejected for scope; current assumptions explicitly call for permanent deletion and simplified recovery.

## Key implementation decisions

- Use a dedicated `Document` entity for file metadata, `DocumentShare` for explicit sharing, and `DocumentActivity` for audit records.
- Model tags as a queryable collection via a normalized `DocumentTag` entity rather than a comma-separated string field.
- Support task attachments with a bridging entity `TaskDocument` so documents may be attached to tasks without duplicating file records.
- Reuse existing notification infrastructure for in-app alerts and document sharing notifications.
