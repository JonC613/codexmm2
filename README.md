# ManualMaster

ManualMaster is a full-stack reference application for organising product manuals. It combines an ASP.NET Core 8 Web API, PostgreSQL storage, PDF text extraction, QR code generation/decoding, and a React 18 frontend built with Vite.

## Features

- Upload PDFs or text instructions, with automatic PDF text extraction
- Store metadata, tags, and original binaries in PostgreSQL (`BYTEA`)
- Full-text search powered by PostgreSQL `to_tsvector`
- Auto-find stub that demonstrates how to integrate web lookups for manuals
- QR code generation (backend) and scanning (frontend with ZXing)
- React dashboard with filters, manual detail view, and mobile-friendly layout

## Project Structure

```
backend/ManualMaster.Api/           # ASP.NET Core API project
backend/ManualMaster.Api.Tests/     # xUnit tests for backend services
frontend/                           # Vite + React SPA
ManualMaster.sln                    # Solution referencing API and tests
```

## Backend Setup

1. Ensure .NET 8 SDK and PostgreSQL 14+ are installed.
2. Create a database (default connection string expects `manualmaster` database and user).
3. Apply the initial schema:
   ```bash
   psql -d manualmaster -f backend/ManualMaster.Api/Migrations/InitialCreate.sql
   ```
4. Set the connection string using either `appsettings.Development.json` or environment variables (e.g. `DATABASE_URL`).
5. Run the API:
   ```bash
   dotnet run --project backend/ManualMaster.Api/ManualMaster.Api.csproj
   ```
   The service listens on `https://localhost:7180` and `http://localhost:5180` by default.

### Configuration

- `ConnectionStrings:Default` – PostgreSQL connection string.
- `DATABASE_URL` – optional override (Heroku-style `postgres://` strings supported by Npgsql).
- `Cors:AllowedOrigins` – array of origins allowed for browser requests (defaults to React dev server).

### Seeding

On startup the API migrates the database and seeds a couple of sample manuals if the `manuals` table is empty. Adjust or remove `ManualDbSeeder` as desired.

### Tests

Run backend unit tests with:
```bash
dotnet test ManualMaster.sln
```

## Frontend Setup

1. Install Node.js 18+.
2. Install dependencies:
   ```bash
   cd frontend
   npm install
   ```
3. Configure the API base URL:
   ```bash
   cp .env.example .env
   # update VITE_API_BASE if the API runs on a different origin
   ```
4. Start the dev server:
   ```bash
   npm run dev
   ```
   Vite listens on `http://localhost:5173`.

### Frontend Tests

Execute the example component tests via:
```bash
npm test
```

## QR Code Workflow

- **Generate**: In the manual detail panel select “QR Code” to request a backend-generated PNG (configurable payload defaults to the manual source URL).
- **Scan**: Use the QR scanner widget to decode an image via ZXing (client-side) with a backend fallback. Successful scans populate the search field.

## Auto-Find Manuals

The `POST /api/manuals/auto-find` endpoint currently returns curated sample data to illustrate the workflow. Replace the implementation in `AutoFindService` with real search/scraping logic (e.g., using external APIs or curated repositories) before production use.

## PDF Text Extraction

PDF uploads are processed with `UglyToad.PdfPig`. Text files overwrite the content field automatically, providing an immediate preview in the UI.

## Database Schema

A raw SQL migration is available at `backend/ManualMaster.Api/Migrations/InitialCreate.sql`. The schema mirrors the specification and creates full-text indexes on manual titles and content.

## Security Notes

- Validate and sanitize external sources when implementing the real auto-find workflow.
- Tighten CORS and authentication for production deployments.
- Move large binaries to object storage (S3, Azure Blob, etc.) and store references if manuals exceed the configured size limits (currently 25 MB).

## Troubleshooting

- If QR decoding fails on non-Windows environments, ensure the required native dependencies for ImageSharp are present.
- PostgreSQL must have the `pg_trgm` extension enabled for trigram indexes; enable with `CREATE EXTENSION IF NOT EXISTS pg_trgm;`.

Happy manual managing!
