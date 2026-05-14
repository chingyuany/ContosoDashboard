# Quickstart: Document Upload and Management

## What this feature adds

- Secure document upload with metadata capture and file validation.
- Local filesystem storage behind `IFileStorageService`.
- Permissions-based download and sharing with IDOR protection.
- Search, preview, edit metadata, and attach documents to tasks.
- In-app notifications for shared documents and project uploads.

## Implementation steps

1. Add `Document`, `DocumentTag`, `DocumentShare`, `DocumentActivity`, and `TaskDocument` models.
2. Update `ContosoDashboard/Data/ApplicationDbContext.cs` to register new entities and configure relationships.
3. Add `IFileStorageService` and `LocalFileStorageService`.
4. Add `DocumentService` to encapsulate upload, authorization, search, sharing, and activity logging.
5. Add or extend Blazor pages for:
   - Documents overview
   - My Documents
   - Project Documents
   - Document details / preview
   - Task details attachment flow
6. Add UI components and validation for file upload, metadata form fields, and empty/search states.
7. Update navigation and dashboard widgets to expose recent documents and the new document area.
8. Add or extend notification code for shared document alerts and project upload notifications.
9. Create EF Core migration and update the local database.
10. Test upload, download, authorization, search, and task attachment scenarios.

## Run locally

1. Open the solution at `ContosoDashboard/ContosoDashboard.sln`.
2. Run the app with:
   ```bash
   dotnet run --project ContosoDashboard/ContosoDashboard.csproj
   ```
3. Open the browser at the local URL shown in the console.
4. Use the existing app login flow and navigate to the Documents page.
5. Upload a supported file (PDF, Office, text, image), verify metadata is recorded, and confirm download and search behavior.

## Configuration

- Store uploaded files under `AppData/uploads` in the app working directory.
- Configure storage location in `appsettings.json` if needed under a new `FileStorage` section.
- Ensure the app has read/write permission to the upload folder.
