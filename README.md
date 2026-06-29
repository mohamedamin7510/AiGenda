# AiGenda API

AiGenda API is a **.NET 9 ASP.NET Core Web API** backend for productivity, collaboration, and AI-enabled workflow management. It provides secure authentication, workspace and task management, note handling, focus-session tracking, background notifications, and external service integrations exposed through Swagger.

## Project Goals

- Deliver a secure and scalable backend for the AiGenda platform.
- Manage workspaces, spaces, tasks, subtasks, notes, and focus sessions.
- Support JWT-based authentication and permission-based authorization.
- Provide a clean REST API for client applications such as Flutter.
- Support external integrations for AI-related workflows and connected services.
- Keep the codebase maintainable through layered design and clear separation of concerns.

## Key Features

- ASP.NET Core Web API on .NET 9
- SQL Server with Entity Framework Core
- ASP.NET Core Identity
- JWT authentication and refresh tokens
- Role-based and permission-based authorization
- Workspace, space, task, subtask, note, and focus-session management
- Soft delete and restore flows for selected entities
- File upload support for avatars and note assets
- Background jobs with Hangfire
- Email sending with MailKit
- Structured logging with Serilog
- Validation with FluentValidation
- Health checks
- Rate limiting
- Swagger / OpenAPI support
- External integration endpoints for Gmail, GitHub, Calendar, and app connections

---

## Table of Contents

- [Introduction](#introduction)
- [Architecture](#architecture)
- [Technologies Used](#technologies-used)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Database](#database)
- [Running the Project](#running-the-project)
- [API Documentation](#api-documentation)
- [Authentication & Authorization](#authentication--authorization)
- [Project Structure](#project-structure)
- [Code Flow](#code-flow)
- [Business Logic](#business-logic)
- [Dependencies](#dependencies)
- [Logging](#logging)
- [Validation](#validation)
- [Error Handling](#error-handling)
- [Testing](#testing)
- [Deployment](#deployment)
- [Performance Considerations](#performance-considerations)
- [Security Considerations](#security-considerations)
- [Troubleshooting](#troubleshooting)
- [Contributing Guide](#contributing-guide)
- [Future Improvements](#future-improvements)
- [Contact](#contact)

---

## Introduction

AiGenda API is the backend service for a productivity-oriented application designed to help users organize work, manage tasks, track focus sessions, and collaborate inside workspaces.

The backend is built as a modular ASP.NET Core application with:
- a controller layer for HTTP endpoints,
- a service layer for business logic,
- Entity Framework Core for persistence,
- ASP.NET Core Identity for user management,
- JWT for stateless authentication,
- and a set of supporting services for email, background jobs, health checks, and external integrations.

The solution is targeting **.NET 9**.

---

## Architecture

### Architecture Style

The project uses a **layered architecture** with the following responsibilities:

- **Controllers**: accept HTTP requests and return responses
- **Services**: implement business rules and orchestration
- **Contracts**: define request and response models
- **Entities**: represent database models
- **Persistence**: configure EF Core and the database context
- **Authentication**: manage JWT tokens, claims, and policies
- **Shared abstractions**: define reusable constants, enums, and result patterns


### Folder Responsibilities

- `Controllers/`  
  Exposes the API endpoints for auth, profiles, roles, workspaces, spaces, tasks, subtasks, notes, and focus sessions.

- `Services/`  
  Contains application logic and data orchestration.

- `Contracts/`  
  Contains DTOs, request/response models, and validators.

- `Entities/`  
  Contains EF Core entities and navigation properties.

- `Presistiences/`  
  Contains the database context and Fluent API entity configurations.

- `Authentication/`  
  Contains JWT provider logic, JWT settings, and custom authorization policies.

- `Abstractions/`  
  Contains constants, enums, error/result helpers, and shared models.

- `Errors/`  
  Contains centralized domain and application error definitions.

- `Extenstions/`  
  Contains custom extension methods used across the project.

- `HealthChecks/`  
  Contains custom health check implementations.

- `Helpers/`  
  Contains utility classes such as email body builders.

- `Hubs/`  
  Contains real-time communication components if used.

- `Templates/`  
  Contains HTML email templates.

- `Migrations/`  
  Contains EF Core migrations for database versioning.

- `wwwroot/`  
  Contains static files and uploaded assets.

### Design Patterns Used

- Dependency Injection
- Service Layer
- DTO pattern
- Result-based response handling
- Fluent Validation
- Custom authorization policies
- Soft delete and restore workflows
- Audit logging through overridden `SaveChanges`

### Request Flow
flowchart LR A["HTTP Request"] --> B["Controller"] B --> C["Validation"] C --> D["Service Layer"] D --> E["AppContext / EF Core"] E --> F["SQL Server"] F --> E E --> D D --> B B --> G["HTTP Response"]

---

## Technologies Used

### Language
- C#

### Frameworks
- ASP.NET Core Web API
- ASP.NET Core Identity
- Entity Framework Core

### Libraries
- Hangfire
- MailKit
- Mapster
- FluentValidation
- Serilog.AspNetCore
- Google.Apis.Auth
- System.Linq.Dynamic.Core
- SharpGrip.FluentValidation.AutoValidation.Mvc
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.AspNetCore.OpenApi
- AspNetCore.HealthChecks.* packages

### Database
- SQL Server

### Tools
- Visual Studio
- Swagger / OpenAPI
- EF Core Migrations
- Hangfire Dashboard
- Health Checks

---

## Prerequisites

Before running the project, make sure the following are installed:

- .NET 9 SDK
- SQL Server
- Visual Studio 2026 or compatible version
- Git

Optional but recommended:
- SQL Server Management Studio
- Postman
- Flutter client for frontend integration testing

---

## Installation

### Clone the Repository
[https://github.com/mohamedamin7510/AiGenda.git](https://github.com/mohamedamin7510/AiGenda.git)

### Restore Dependencies
`dotnet restore`

### Build the Project
`dotnet build`

### Apply Database Migrations
`dotnet ef database update`

### Run The Application 
`dotnet run`


---

## Configuration

Configuration is primarily defined in:

- `appsettings.json`
- `appsettings.Development.json`
- `user secrets`

### Important Configuration Sections

#### Connection Strings
The project uses two SQL Server databases:
- `application database`
- `Hangfire database`

Configured keys:
- `ConnectionStrings:connectionstring`
- `ConnectionStrings:HangfireConnection`

#### JWT Settings
- `Jwt:SymmetricKey`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:ExpirtMiniuites`

#### CORS
- `AllowedOrigins`

#### Email Settings
- `MailSettings:DisplayName`
- `MailSettings:Host`
- `MailSettings:User`
- `MailSettings:Pass`
- `MailSettings:port`

#### Google Authentication
- `Authentication:Google:AllowedClientIds`
- `Authentication:Google:ClientSecret`

#### Rate Limiting
- `RateLimitingTokens:PolicyName`
- `RateLimitingTokens:PerLimit`
- `RateLimitingTokens:PerLimitPeriod`

### Secrets

Some sensitive values are empty or placeholder values in the source code snapshot, especially:
- JWT symmetric key
- email password
- Hangfire dashboard credentials
- Google client secret

These should be stored securely using:
- user secrets in development
- environment variables in production
- a secret manager or cloud key vault in production environments

---

## Database

### Database Provider
- SQL Server

### Context
The main database context is `AppContext`, which inherits from:

- `IdentityDbContext<ExtendedUser, ApplicationRole, string>`

### Main Entity Sets

The context includes:
- `Users`
- `WorkSpaces`
- `WorkspaceMembers`
- `Spaces`
- `Tasks`
- `TaskAssignees`
- `SubTasks`
- `Notes`
- `NoteAssets`
- `TextNoteContents`
- `VoiceNoteContents`
- `ImageNoteContents`
- `HandDrawNoteContents`
- `FocusSessions`

### Migrations

The repository contains multiple EF Core migrations, which indicates that the schema has been evolved gradually over time.

### Seed Data

No explicit seed-data implementation was found in the inspected source snapshot.

### Updating the Database

Use standard EF Core commands:
`dotnet ef migrations add <MigrationName> dotnet ef database update`

---

## Running the Project

### Visual Studio

Open the solution in Visual Studio and run the project using the standard debug profile.

### CLI
dotnet run

### Health Endpoint

The application exposes a health check endpoint:

- `/health`

### Swagger

Swagger/OpenAPI is enabled for exploring the endpoints and testing requests.

---

## API Documentation

> Note: The following API summary is based on the inspected source files.  
> The deployed Swagger UI also shows additional integration endpoints for external services, but their controller source was not included in the current repository snapshot. For those endpoints, request/response contracts could not be fully verified from source.

---

### Authentication APIs

| Method | Route | Auth | Description |
|---|---|---:|---|
| POST | `/api/Auth` | No | Login and receive JWT |
| PUT | `/api/Auth/refresh` | No | Refresh access token |
| PUT | `/api/Auth/revoke-refresh-token` | No | Revoke refresh token |
| POST | `/api/Auth/register` | No | Register a new user |
| POST | `/api/Auth/confirm-email` | No | Confirm email |
| POST | `/api/Auth/resend-confirm-email` | No | Resend confirmation email |
| POST | `/api/Auth/forget-password` | No | Request password reset |
| PUT | `/api/Auth/reset-password` | No | Reset password |
| POST | `/api/Auth/google` | No | Google login |

### Profile APIs

Base route: `/me`

| Method | Route | Auth | Description |
|---|---|---:|---|
| GET | `/me` | Yes | Get current profile |
| PUT | `/me` | Yes | Update current profile |
| PUT | `/me/change-password` | Yes | Change password |
| POST | `/me/change-email` | Yes | Start email change process |
| PUT | `/me/confirm-change-email` | Yes | Confirm new email |
| POST | `/me/avatar` | Yes | Upload avatar |
| GET | `/me/avatar` | Yes | Get avatar |
| DELETE | `/me/avatar` | Yes | Delete avatar |

### Roles APIs

Base route: `/api/Roles`

| Method | Route | Auth | Description |
|---|---|---:|---|
| GET | `/api/Roles` | Admin | List roles |
| GET | `/api/Roles/{Id}` | Admin | Get a role |
| POST | `/api/Roles` | Admin | Create a role |
| PUT | `/api/Roles/{Id}` | Admin | Update a role |
| PUT | `/api/Roles/{Id}/toggle-status` | Admin | Toggle role status |

### WorkSpaces APIs

Base route: `/api/WorkSpaces`

| Method | Route | Auth | Description |
|---|---|---:|---|
| POST | `/api/WorkSpaces` | Member | Create workspace |
| GET | `/api/WorkSpaces` | Member | Get all workspaces |
| GET | `/api/WorkSpaces/{Id}` | Member | Get workspace by id |
| GET | `/api/WorkSpaces/dashboard` | Member | Get dashboard data |
| GET | `/api/WorkSpaces/{Id}/members` | Member | Get members |
| GET | `/api/WorkSpaces/{Id}/members/{MemberUserId}/permissions` | Member | Get member permissions |
| POST | `/api/WorkSpaces/{Id}/member` | Member + permission | Add member |
| PUT | `/api/WorkSpaces/{Id}` | Member + permission | Update workspace |
| PUT | `/api/WorkSpaces/{Id}/restore` | Member + permission | Restore workspace |
| PUT | `/api/WorkSpaces/{Id}/members/{MemberUserId}/permissions` | Member + permission | Update permissions |
| DELETE | `/api/WorkSpaces/{Id}` | Member + permission | Delete workspace |
| DELETE | `/api/WorkSpaces/{Id}/remove-member` | Member | Remove member |
| GET | `/api/WorkSpaces/deleted` | Member | Get deleted workspaces |
| GET | `/api/WorkSpaces/{Id:int}/dashboard` | Member + permission | Get workspace dashboard by id |

### Spaces APIs

Base route: `/api/WorkSpaces/{WorkspaceId:int}/Spaces`

| Method | Route | Auth | Description |
|---|---|---:|---|
| POST | `/api/WorkSpaces/{WorkspaceId:int}/Spaces` | Member + permission | Create space |
| GET | `/api/WorkSpaces/{WorkspaceId:int}/Spaces` | Member + permission | List spaces |
| GET | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/deleted` | Member + permission | List deleted spaces |
| GET | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{Id}` | Member + permission | Get space by id |
| PUT | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{Id}` | Member + permission | Update space |
| PUT | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{Id}/move` | Member + permission | Move space |
| DELETE | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{Id}` | Member + permission | Delete space |
| PUT | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{Id}/restore` | Member + permission | Restore space |
| GET | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{Id}/analytics` | Member | Get analytics |
| GET | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{Id}/analytics/export` | Member | Export analytics |

### Tasks APIs

Base route: `/api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks`

| Method | Route | Auth | Description |
|---|---|---:|---|
| POST | `/api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks` | Member + permission | Create task |
| GET | `/api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks` | Member + permission | List tasks |
| GET | `/api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks/{Id}` | Member + permission | Get task |
| GET | `/api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks/removed` | Member + permission | List removed tasks |
| GET | `/api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks/removed/{Id}` | Member + permission | Get removed task |
| PUT | `/api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks/{Id}` | Member + permission | Update task |
| PUT | `/api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks/{Id}/status` | Member + permission | Update task status |
| DELETE | `/api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks/{Id}` | Member + permission | Delete task |
| PUT | `/api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks/{Id}/restore` | Member + permission | Restore task |
| PUT | `/api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks/{Id}/assign` | Member + permission | Assign member to task |
| DELETE | `/api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks/{Id}/unassign` | Member + permission | Unassign member |

### SubTasks APIs

Base route: `/api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks/{TaskId}/SubTasks`

| Method | Route | Auth | Description |
|---|---|---:|---|
| POST | `/api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks/{TaskId}/SubTasks` | Member + permission | Create subtask |
| PUT | `/api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks/{TaskId}/SubTasks/{Id}` | Member + permission | Update subtask |
| PUT | `/api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks/{TaskId}/SubTasks/{Id}/status` | Member + permission | Update subtask status |
| DELETE | `/api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks/{TaskId}/SubTasks/{Id}` | Member + permission | Delete subtask |

### Notes APIs

Base route: `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{SpaceId}/Notes`

| Method | Route | Auth | Description |
|---|---|---:|---|
| POST | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{SpaceId}/Notes` | Member + permission | Create note |
| GET | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{SpaceId}/Notes` | Member + permission | List notes |
| GET | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{SpaceId}/Notes/{Id}` | Member + permission | Get note |
| PUT | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{SpaceId}/Notes/{Id}` | Member + permission | Update note |
| DELETE | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{SpaceId}/Notes/{Id}` | Member + permission | Delete note |

### Focus Sessions APIs

Base route: `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{SpaceId}/Tasks/{TaskId}/FocusSessions`

| Method | Route | Auth | Description |
|---|---|---:|---|
| POST | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{SpaceId}/Tasks/{TaskId}/FocusSessions` | Member + permission | Start focus session |
| GET | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{SpaceId}/Tasks/{TaskId}/FocusSessions/current` | Member + permission | Get current session |
| PUT | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{SpaceId}/Tasks/{TaskId}/FocusSessions/{SessionId}/pause` | Member + permission | Pause session |
| PUT | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{SpaceId}/Tasks/{TaskId}/FocusSessions/{SessionId}/resume` | Member + permission | Resume session |
| PUT | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{SpaceId}/Tasks/{TaskId}/FocusSessions/{SessionId}/complete` | Member + permission | Complete session |
| PUT | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{SpaceId}/Tasks/{TaskId}/FocusSessions/{SessionId}/abandon` | Member + permission | Abandon session |
| PUT | `/api/WorkSpaces/{WorkspaceId:int}/Spaces/{SpaceId}/Tasks/{TaskId}/FocusSessions/{SessionId}/subtasks/{SubTaskId}` | Member + permission | Toggle session subtask |

### External Integration and AI-Related Endpoints

The deployed Swagger UI shows integration-related endpoints such as:

- `/integrations/v1/status`
- `/integrations/v1/connect/{service}`
- `/integrations/v1/github/issues`
- `/integrations/v1/github/issues/close`
- `/integrations/v1/github/prs`
- `/integrations/v1/github/repos`
- `/integrations/v1/gmail/inbox`
- `/integrations/v1/gmail/message`
- `/integrations/v1/gmail/send`
- `/integrations/v1/gmail/reply`
- `/integrations/v1/gmail/draft`
- `/integrations/v1/calendar/events`
- `/api/users/current/app-connections`
- `/api/users/current/app-connections/{connectionId}`
- `/api/users/current/app-connections/authorize/{provider}`
- `/api/users/current/app-connections/{connectionId}/sync`

These routes are visible in Swagger, but the corresponding controller source was not present in the current inspected snapshot. Because of that, exact request bodies and response contracts could not be fully determined from the source code.

---

## Authentication & Authorization

### Authentication

- JWT bearer authentication is used.
- The JWT token includes user identity, email, name, roles, and permissions.
- Refresh tokens are supported.

### Authorization

- Role-based authorization is used for high-level access control.
- Permission-based authorization is used for fine-grained feature control.
- The `HasPermission` attribute is used extensively in controllers.

### Identity Rules

Configured identity rules include:
- confirmed email required
- unique email required
- minimum password length of 8

---

## Project Structure

### Important Directories

- `Controllers/`  
  HTTP API layer.

- `Services/`  
  Business logic and orchestration.

- `Contracts/`  
  DTOs and validators.

- `Entities/`  
  Domain and persistence entities.

- `Presistiences/`  
  Database context and EF Core configuration.

- `Authentication/`  
  JWT and authorization support.

- `Abstractions/`  
  Constants, enums, result patterns, and shared helpers.

- `Errors/`  
  Error definitions.

- `Extenstions/`  
  Shared extension methods.

- `HealthChecks/`  
  Health check implementations.

- `Helpers/`  
  Utility helpers.

- `Templates/`  
  Email templates.

- `Migrations/`  
  EF Core schema history.

- `wwwroot/`  
  Uploaded files and static assets.

---

## Code Flow

A typical request follows this path:

1. The client sends an HTTP request.
2. A controller receives the request.
3. Request DTO validation runs automatically.
4. The controller calls the relevant service.
5. The service applies business rules.
6. The service reads or writes data through `AppContext`.
7. Entity Framework Core interacts with SQL Server.
8. The result is returned through the service.
9. The controller converts it to an HTTP response.

### Request Flow Diagram

---

## Business Logic

The backend handles the main application workflows:

### Users and Authentication
- register users
- login
- refresh tokens
- revoke refresh tokens
- email confirmation
- password reset
- Google login

### Profiles
- view profile
- update profile
- change password
- change email
- upload avatar
- remove avatar

### Workspaces
- create and manage workspaces
- add or remove members
- manage member permissions
- restore or delete workspaces
- load dashboard data

### Spaces
- create spaces inside workspaces
- update, move, delete, and restore spaces
- generate analytics and export reports

### Tasks
- create and update tasks
- update status
- assign and unassign members
- soft delete and restore tasks

### Subtasks
- create subtasks
- update subtask state
- delete subtasks

### Notes
- support text, voice, image, and hand-drawn notes
- support uploads and attachments

### Focus Sessions
- start a session
- pause, resume, complete, or abandon a session
- toggle subtasks during a focus session

### External Integrations
The current system also exposes integration screens in Swagger for connected services such as:
- Gmail
- GitHub
- Calendar
- app connections and synchronization

This supports the broader AI-enabled workflow of the application.

---

## Dependencies

### Core Dependencies and Purpose

- `Microsoft.EntityFrameworkCore.SqlServer`  
  SQL Server persistence.

- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`  
  User and role management.

- `Microsoft.AspNetCore.Authentication.JwtBearer`  
  JWT token validation.

- `FluentValidation`  
  Input validation.

- `SharpGrip.FluentValidation.AutoValidation.Mvc`  
  Automatic MVC validation.

- `Mapster`  
  Object mapping.

- `Serilog.AspNetCore`  
  Structured logging.

- `Hangfire`  
  Background job processing.

- `MailKit`  
  Email delivery.

- `Google.Apis.Auth`  
  Google authentication support.

- `AspNetCore.HealthChecks.*`  
  Health checks for SQL Server and Hangfire.

- `Swashbuckle.AspNetCore.SwaggerUI` and `Microsoft.AspNetCore.OpenApi`  
  API documentation.

- `System.Linq.Dynamic.Core`  
  Dynamic querying support.

---

## Logging

Logging is configured with **Serilog**.

### Current Behavior
- Logs are written to the console.
- File logging is prepared in configuration but commented out.
- Logs are enriched with:
  - machine name
  - process id
  - thread id

### Notes
For production, logging can be extended to file storage or a centralized logging platform.

---

## Validation

Validation is implemented with **FluentValidation**.

### Observed Pattern
- Requests have dedicated validator classes.
- Auto-validation is enabled for MVC.

### Examples
- `LoginReqValidator`
- `RegisterRequestValidator`
- `TaskRequestValidator`
- `SpaceRequestValidator`
- `NoteRequestValidator`

---

## Error Handling

The application uses:
- `GlobalExceptionHandler`
- `ProblemDetails`
- result-based response conversion using `ToProblem()`

### Behavior
- Successful operations return standard success responses.
- Validation or business failures return structured error responses.
- Controllers remain thin and do not duplicate error logic.

---

## Testing

### Automated Tests
No dedicated test project was found in the current repository snapshot.

### Manual Testing
The backend can be tested through:
- Swagger UI
- Postman
- browser requests for GET endpoints

### Recommended Test Areas
- authentication
- workspace management
- task CRUD
- notes upload
- focus session behavior
- integration endpoints
- rate limiting behavior

---

## Deployment

### Production Deployment
The application can be deployed to:
- IIS
- Linux with Kestrel and Nginx
- Azure App Service or similar hosting platforms

### Docker
No Docker configuration was found in the current source snapshot.

### IIS
The project can be published and hosted in IIS as a standard ASP.NET Core application.

### Linux
The app can be published for Linux and hosted behind a reverse proxy.

---

## Performance Considerations

- Rate limiting is enabled.
- Background jobs reduce pressure on request-time processing.
- Soft delete avoids unnecessary hard deletes.
- EF Core configurations prevent unintended cascade delete behavior.
- CORS is restricted to configured origins.

---

## Security Considerations

- JWT authentication
- Refresh token support
- Role-based authorization
- Permission-based authorization
- Email confirmation requirement
- Rate limiting
- Restricted CORS policy
- External integration authorization
- Secret values should be stored securely outside source control

---

## Troubleshooting

### Database Connection Errors
Check:
- SQL Server is running
- connection strings are correct
- migrations were applied

### JWT Authentication Fails
Check:
- JWT symmetric key
- issuer
- audience
- token expiry

### Email Sending Fails
Check:
- SMTP host
- username/password
- port
- TLS settings

### CORS Problems
Check:
- `AllowedOrigins`

### Hangfire Jobs Do Not Run
Check:
- Hangfire connection string
- Hangfire server startup
- dashboard auth settings

### Swagger Does Not Open
Check:
- application is running
- OpenAPI is enabled
- the correct profile is selected

---

## Contributing Guide

### Coding Conventions
- Keep controllers thin.
- Put business logic in services.
- Use DTOs for requests and responses.
- Use validators for input checks.
- Return result-based responses consistently.
- Maintain the layered structure.

### Branching Strategy
A practical strategy is:
- `main` for stable code
- feature branches for development work
- pull requests for review

### Commit Messages
Use descriptive commit messages such as:
- `feat: add task restore endpoint`
- `fix: handle invalid refresh token`
- `refactor: improve workspace service mapping`

### Pull Requests
Include:
- summary of changes
- reason for the change
- testing performed
- screenshots for Swagger or UI changes if applicable

---

## Future Improvements

- Add automated unit tests
- Add integration tests
- Add Redis caching
- Improve observability and monitoring
- Expand integration documentation
- Add Docker support
- Strengthen secret management
- Add API versioning if needed
- Extend AI orchestration features

---


## Contact

Repository: `https://github.com/mohamedamin7510/AiGenda`

---

## Notes

- The README above is based on the current source snapshot and the Swagger screenshots provided.
- Some external integration endpoints were visible in Swagger, but their controller source was not included in the inspected files.
- If you want, the next step can be a **shorter GitHub README version** optimized for the repository homepage.
