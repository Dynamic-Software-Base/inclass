# Keycloak Infrastructure

This folder contains the Keycloak identity provider setup for the InClass application.

## Local Development Setup

### Prerequisites
- Docker Desktop installed and running
- Ports 8080 and 5433 available

### Quick Start

1. **Start Keycloak locally:**
   ```bash
   cd infra/keycloak
   docker-compose up -d
   ```

2. **Wait for Keycloak to be ready** (takes ~30-60 seconds):
   ```bash
   docker-compose logs -f keycloak
   # Wait until you see "Keycloak 23.0 started"
   ```

3. **Access Keycloak Admin Console:**
    - URL: http://localhost:8080
    - Username: `admin`
    - Password: `admin`

4. **Stop Keycloak:**
   ```bash
   docker-compose down
   ```

5. **Stop and clear all data** (reset to fresh install):
   ```bash
   docker-compose down -v
   ```

## Realm Configuration

The `realm-export.json` file contains the complete Keycloak configuration for the InClass application. This includes:
- Clients (BFF, API)
- Roles (school_owner, teacher, student, parent)
- Authentication flows
- Email settings
- Token settings

### Exporting Realm Changes

After making changes in the Keycloak Admin UI:

1. Go to Realm Settings → Action → Partial Export
2. Check all boxes (Include groups, clients, roles)
3. Download the JSON file
4. Replace `realm-export.json` in this folder
5. Commit to Git

### Importing Realm (Automatic)

The realm is automatically imported when Keycloak starts. If you need to force re-import:

```bash
docker-compose down -v
docker-compose up -d
```

## Staging/Production Deployment

See `Dockerfile` and Railway deployment instructions below.

### Railway Setup

1. Create new Railway project
2. Add Postgres service (or use external Neon DB)
3. Add Keycloak service from Docker image
4. Set environment variables (see Railway section below)

Environment variables needed:
```
KC_DB=postgres
KC_DB_URL=jdbc:postgresql://<host>:<port>/<database>?sslmode=require
KC_DB_USERNAME=<username>
KC_DB_PASSWORD=<password>
KEYCLOAK_ADMIN=<admin-username>
KEYCLOAK_ADMIN_PASSWORD=<secure-password>
KC_HOSTNAME=<your-railway-domain>
KC_HOSTNAME_STRICT=true
KC_PROXY=edge
```

## Troubleshooting

**Keycloak won't start:**
- Check if ports 8080 or 5433 are already in use
- Check Docker logs: `docker-compose logs keycloak`

**Database connection errors:**
- Ensure Postgres container is healthy: `docker-compose ps`
- Check logs: `docker-compose logs postgres`

**Can't login to admin console:**
- Default credentials are admin/admin
- If you changed them, check your .env file

## Folder Structure

```
infra/keycloak/
├── docker-compose.yml       # Local development setup
├── Dockerfile              # Production/staging image (coming soon)
├── realm-export.json       # Keycloak configuration (coming soon)
├── .env.example            # Environment variable template
├── .gitignore             # Git ignore rules
└── README.md              # This file
```
