# Document Service Contract

## Purpose

Defines the public contract for document management services and file storage in ContosoDashboard. This contract guides implementation and verifies that business logic is exposed consistently for pages and potential reuse.

## `IFileStorageService`

### Responsibilities

- Save uploaded files outside the web-accessible directory.
- Delete stored files when documents are removed.
- Return secure stream access for file downloads.

### Surface

- `Task<string> SaveFileAsync(Stream content, string fileName, string contentType)`
  - Returns the stored secure filename or key.
- `Task<Stream> OpenReadAsync(string storedFileName)`
  - Returns a read-only stream for downloads or preview.
- `Task<bool> DeleteFileAsync(string storedFileName)`
  - Deletes the stored file and returns success status.
- `Task<bool> FileExistsAsync(string storedFileName)`
  - Returns whether the file exists in storage.

## `IDocumentService`

### Responsibilities

- Validate file uploads and metadata.
- Enforce authorization for uploads, downloads, edits, deletes, and shares.
- Search documents by title, description, tags, uploader, and project.
- Record audit events for document operations.
- Support task attachment workflows.

### Surface

- `Task<Document> UploadDocumentAsync(DocumentUploadRequest request, ClaimsPrincipal user)`
- `Task<Document> GetDocumentDetailsAsync(int documentId, ClaimsPrincipal user)`
- `Task<Stream> DownloadDocumentAsync(int documentId, ClaimsPrincipal user)`
- `Task<IEnumerable<Document>> SearchDocumentsAsync(DocumentSearchCriteria criteria, ClaimsPrincipal user)`
- `Task<Document> UpdateDocumentMetadataAsync(int documentId, DocumentMetadataUpdate update, ClaimsPrincipal user)`
- `Task<bool> DeleteDocumentAsync(int documentId, ClaimsPrincipal user)`
- `Task<bool> ShareDocumentAsync(int documentId, int recipientUserId, string notes, ClaimsPrincipal user)`
- `Task<IEnumerable<Document>> GetSharedWithMeDocumentsAsync(ClaimsPrincipal user)`
- `Task<bool> AttachDocumentToTaskAsync(int documentId, int taskId, ClaimsPrincipal user)`

## Page contract

### Documents page flow

- `Documents.razor` displays:
  - My Documents list
  - Project Documents list
  - Shared with Me list
  - Search bar and filters
  - Upload button with metadata form
- Pages call `IDocumentService` and present only authorized documents.
- Empty states must show a clear message and suggested actions.

### Upload workflow

- Page collects required metadata: title, category, optional description, optional project, optional tags.
- Page sends upload form data and file content to `UploadDocumentAsync`.
- Validation errors are surfaced to the user with clear messages.
