# ContosoDashboard Constitution

<!-- Sync Impact Report: Initial ratification v1.0.0 | May 14, 2026
   - Status: New constitution created with 5 core principles
   - Ratified: 2026-05-14
   - Principles: Spec-Driven Development, Training-First Quality, Security by Design, Clean Architecture, Offline-First Design
   - Templates requiring updates: plan-template.md (verify Constitution Check section), spec-template.md (verify scope alignment)
   - No prior versions to migrate from
-->

## Core Principles

### I. Spec-Driven Development (Non-Negotiable)

Every feature MUST be specified before implementation. Specifications define user scenarios, requirements, and acceptance criteria.
All code changes must trace back to either a specification or a documented architectural decision. The specification MUST include testable acceptance scenarios and explicit requirements. No feature is considered complete without its specification being approved and maintained.

### II. Training-First Quality

This is a training project; clarity and educational value MUST be prioritized alongside functionality. All code MUST be readable, well-commented for learning purposes, and designed to demonstrate best practices in a simplified context. Architecture MUST document its reasoning—why patterns were chosen, what trade-offs were made, and how this relates to production systems. Documentation MUST explain not just what the code does, but why it was done this way for training benefit.

### III. Security by Design

Security is built in from the start, not bolted on. All features MUST implement role-based access control (RBAC) with explicit authorization checks at service level, not just page level. Defense in depth is mandatory: authorize at multiple layers (middleware, page attributes, services). Protection against common vulnerabilities (IDOR, privilege escalation, unauthorized data access) MUST be verified in code review. Even though mock authentication is used for training, the security architecture MUST be production-grade in design and ready for real identity providers.

### IV. Clean Architecture with Separation of Concerns

Code MUST follow strict separation: Models (entities only), Services (business logic), Data (database context), Pages/Controllers (UI/routing). Each layer MUST be independently testable. Services MUST contain all authorization and validation logic. Page/controller code MUST be thin and focused on presentation only. Dependencies MUST flow inward (Pages → Services → Models → Data). Circular dependencies are forbidden.

### V. Offline-First with Cloud Migration Path

The application MUST work completely offline with local SQL Server LocalDB. All external dependencies (file storage, messaging, etc.) MUST be abstracted behind interfaces (IFileStorageService, etc.) with local implementations. This enables seamless migration to Azure services: Azure Blob Storage, Service Bus, etc., can replace local implementations without changing business logic, UI, or database schema. The local implementation MUST be the reference implementation; cloud implementations MUST produce identical behavior.

## Architecture & Technical Constraints

### Technology Stack (Fixed)

- **Framework**: ASP.NET Core 8.0
- **UI**: Blazor Server
- **Database**: Entity Framework Core with SQL Server LocalDB
- **Authentication**: Cookie-based (mock for training; production-ready interface design)
- **Authorization**: Claims-based identity with role-based policies
- **Styling**: Bootstrap 5.3 with Bootstrap Icons

### Code Organization

```
ContosoDashboard/
├── Models/          # Entity models only—no logic
├── Services/        # Business logic, validation, authorization
├── Data/            # DbContext, migrations
├── Pages/           # Blazor/Razor pages, thin presentation layer
├── Shared/          # Shared components
└── wwwroot/         # Static assets only
```

### Data Security Requirements

- User isolation: Every query MUST filter by current user's permissions unless explicitly documented
- IDOR prevention: All data access MUST verify user has permission to access that specific resource
- No secrets in code: Use appsettings.json with environment-specific overrides
- Audit logging: All security-relevant actions (login, authorization failures, data access) MUST be logged

## Development Workflow & Quality Gates

### Before Implementation

- Feature specification MUST be approved (user scenarios, requirements, acceptance criteria)
- Architecture decisions MUST be documented if they affect multiple components
- Security implications MUST be reviewed in specification phase

### During Development

- Code MUST follow clean architecture principles (failing this is grounds for rejection)
- Authorization checks MUST be implemented at service level minimum (page/controller level acceptable for defense-in-depth)
- All public methods MUST have clear intent—purpose of the method, parameters, return value, side effects
- Complex logic MUST be documented with "why" not just "what"

### Before Merge

- Specification acceptance scenarios MUST pass (functional testing)
- Security review: IDOR protection verified, authorization at service level, defense in depth present
- Code review: Clean architecture verified, no circular dependencies, comments present for clarity
- For new database models: Migration created and tested; indexes added for queried fields

## Governance

This constitution supersedes all other development practices and guidance for the ContosoDashboard project. All architectural decisions, code changes, and feature implementations MUST align with these five core principles.

### Amendment Process

- Proposed amendments MUST include: current wording, new wording, rationale for change, impact on existing code
- Amendments MUST maintain the five core principles unless explicitly proposing principle-level changes
- Minor clarifications (wording, formatting, examples) use PATCH versioning
- New sections or expanded guidance use MINOR versioning
- Principle changes or backward-incompatible removals use MAJOR versioning
- All amendments require documentation in the Sync Impact Report

### Compliance Review

- All pull requests MUST reference the relevant principle(s) they implement
- Security reviews MUST verify Principle III (Security by Design) compliance
- Architecture reviews MUST verify Principle IV (Clean Architecture) compliance
- Code comments MUST reflect Principle II (Training-First Quality) for clarity

### Related Guidance

For runtime development details and best practices, refer to `.github/copilot-instructions.md` and project documentation.

**Version**: 1.0.0 | **Ratified**: 2026-05-14 | **Last Amended**: 2026-05-14
