# ClickGuard.Admin

ClickGuard.Admin is an ASP.NET Core web app for administrators who manage ClickGuard tenants, invite users, and maintain trusted-domain allowlists. It also serves those allowlists as JSON to ClickGuard agents.

## Features

- Create and list tenants with the `FcsAdmin` role.
- Generate one-time invitation links for `Admin` and `ReadOnly` accounts. Recipients use the link to set a password.
- View tenant details, user counts, and trusted-domain counts with either tenant role.
- Add and remove trusted-domain entries for your tenant with the `Admin` role.
- Retrieve a tenant’s stored domain entries through the agent API.

## Tech Stack

- C# and ASP.NET Core 8 with Razor Pages and an MVC API controller.
- ASP.NET Core Identity for sign-in, sign-out, and role-based access to the admin pages.
- Entity Framework Core 8.0.23 with the SQL Server provider and checked-in migrations. Local settings target SQL Server LocalDB, and production settings target Azure SQL Database.
- Bootstrap 5.3.0 via jsDelivr in the shared layout, with jQuery and validation assets in `wwwroot/lib/`.

## Prerequisites

- Git and the .NET 8 SDK. The project targets `net8.0` and restores packages through NuGet.
- A SQL Server instance and credentials with permission to apply the initial migration. You can use SQL Server Express LocalDB on Windows with the checked-in defaults.
- The `dotnet-ef` CLI for applying migrations. The setup below uses version `8.0.23` to match the project’s EF Core packages.

## Installation & Setup

Run the following commands in PowerShell.

1. Clone the repository and restore dependencies.

   ```powershell
   git clone https://github.com/fcilab2024/ClickGuard.Admin.git
   cd ClickGuard.Admin
   dotnet restore .\ClickGuard.Admin.csproj
   ```

2. Configure the database. [`appsettings.json`](appsettings.json) supplies `ConnectionStrings:DefaultConnection` for `(localdb)\MSSQLLocalDB` using Windows authentication. To use a different database, set `ConnectionStrings__DefaultConnection` to its SQL Server connection string in the same shell where you apply migrations and run the app. This example uses a separate LocalDB database.

   ```powershell
   $env:ConnectionStrings__DefaultConnection = 'Server=(localdb)\MSSQLLocalDB;Database=ClickGuardAdminDev;Trusted_Connection=True;TrustServerCertificate=True;'
   ```

3. Install the EF Core CLI if needed, then apply the initial migration before starting the app. Startup queries the Identity tables to seed roles.

   ```powershell
   dotnet tool install --global dotnet-ef --version 8.0.23
   dotnet ef database update
   ```

4. Trust the ASP.NET Core development certificate for the HTTPS launch profile.

   ```powershell
   dotnet dev-certs https --trust
   ```

<!-- TODO: Confirm and document the deployment URL. -->

## Usage

Start the app and open [https://localhost:7191](https://localhost:7191). The `https` profile sets `ASPNETCORE_ENVIRONMENT` to `Development` and also listens on `http://localhost:5074`.

```powershell
dotnet run --project .\ClickGuard.Admin.csproj --launch-profile https
```

### Admin portal

On startup, the app ensures the `FcsAdmin`, `Admin`, and `ReadOnly` roles exist. It grants `FcsAdmin` to an existing Identity user whose email matches the one in [`Program.cs`](Program.cs). The portal workflow starts with that account already provisioned in the configured database.

<!-- TODO: Document the supported procedure for provisioning the first FcsAdmin account. -->

1. Sign in at `/Identity/Account/Login` as an `FcsAdmin`. Create a tenant at `/Admin/Tenants`.
2. Select **Manage Invites** on the tenant row, enter the recipient’s email, and generate an invite for `Admin` or `ReadOnly`. Open the generated `/Invite` link to set the tenant account’s password.
3. Sign in as the tenant user and open `/Dashboard`. With the `Admin` role, open `/Tenant/TrustedDomains` to add `example.com` or delete an existing entry.

### Agent API

`GET /api/agent/allowlist?tenantId=<guid>` accepts unauthenticated requests and returns the tenant’s stored entries sorted by domain pattern. Copy the tenant GUID from **Manage Invites** and substitute it for the example GUID.

```powershell
$tenantId = '01234567-89ab-cdef-0123-456789abcdef'
Invoke-RestMethod "https://localhost:7191/api/agent/allowlist?tenantId=$tenantId" | ConvertTo-Json
```

Example response after adding `example.com` to that tenant:

```json
{
  "domains": ["example.com"]
}
```

A tenant GUID with no matching entries returns `{"domains":[]}`.

## Project Structure

```text
ClickGuard.Admin/
├── Areas/Identity/    Login and logout pages
├── Controllers/       Agent allowlist API
├── Data/              EF Core context, Identity user, and role seeding
├── Migrations/        SQL Server schema migrations
├── Models/            Tenant, invite, trusted-domain, and API response models
├── Pages/             Portal pages, tenant dashboard, and invite redemption
├── Properties/        Launch profiles and service dependency configuration
├── Services/          Invite token generation and hashing
├── wwwroot/           Static assets and bundled frontend libraries
└── Program.cs         Application startup and service configuration
```

## Development & Testing

Build the project and run the local server with Hot Reload.

```powershell
dotnet build .\ClickGuard.Admin.csproj --configuration Release
dotnet watch run --launch-profile https
```

For a manual smoke test, call the allowlist endpoint before and after adding `example.com` to the same tenant. Confirm that the response includes the new entry.

<!-- TODO: Document automated test and lint commands when those tools are configured. -->

## License

The project license is unspecified. Third-party license notices are included under `wwwroot/lib/`.

<!-- TODO: Confirm the project license and add a root LICENSE file. -->
