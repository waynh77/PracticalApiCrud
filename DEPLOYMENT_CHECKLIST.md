# 📋 Migration & Deployment Checklist

## Pre-Deployment Steps

### 1. Database Preparation
- [ ] Backup existing database
- [ ] Review pending migrations
- [ ] Test migrations on development environment
- [ ] Verify database schema changes

### 2. Configuration
- [ ] Update `appsettings.json` with production values
- [ ] Set JWT secret key (use strong random key)
- [ ] Configure database connection string
- [ ] Set `AllowedOrigins` for CORS
- [ ] Set JWT token expiration

### 3. Code Review
- [ ] Review all code changes
- [ ] Check for console.WriteLine() statements (remove in prod)
- [ ] Verify error messages don't expose sensitive info
- [ ] Check for hardcoded values

### 4. Testing
- [ ] Test authentication (register, login)
- [ ] Test task CRUD operations
- [ ] Test authorization (can't access other user's data)
- [ ] Test error scenarios (invalid input, not found, etc.)
- [ ] Load testing (if high traffic expected)

---

## Database Migration Steps

### Option 1: Using Entity Framework Migrations

```bash
# Step 1: Create migration
dotnet ef migrations add RefactoredAPI -p PracticalBEsesi3 -s PracticalBEsesi3

# Step 2: Review generated migration file
# Location: PracticalBEsesi3/Migrations/[date]_RefactoredAPI.cs

# Step 3: Apply migration to development database
dotnet ef database update -p PracticalBEsesi3 -s PracticalBEsesi3

# Step 4: Generate SQL script (for production review)
dotnet ef migrations script > migration.sql
```

### Option 2: Fresh Database (Development Only)

```bash
# WARNING: This will delete all data!

# Step 1: Remove database
dotnet ef database drop -p PracticalBEsesi3 -s PracticalBEsesi3

# Step 2: Create fresh database
dotnet ef database create -p PracticalBEsesi3 -s PracticalBEsesi3
```

### Option 3: Manual SQL Migration

If using production SQL Server directly:

```sql
-- Add CreatedAt to Users if missing
ALTER TABLE Users
ADD CreatedAt DATETIME DEFAULT GETUTCDATE();

-- Rename UpdateAt to UpdatedAt on TaskItems
EXEC sp_rename 'TaskItems.UpdateAt', 'UpdatedAt', 'COLUMN';

-- Add unique index on Email
CREATE UNIQUE INDEX UX_Users_Email ON Users(Email);

-- Add index on UserId for performance
CREATE INDEX IX_TaskItems_UserId ON TaskItems(UserId);

-- Add foreign key constraint with cascade delete
ALTER TABLE TaskItems
DROP CONSTRAINT FK_TaskItems_Users_UserId;

ALTER TABLE TaskItems
ADD CONSTRAINT FK_TaskItems_Users_UserId
    FOREIGN KEY (UserId)
    REFERENCES Users(Id)
    ON DELETE CASCADE;
```

---

## Deployment to IIS

### Prerequisites
- IIS 10+ installed
- .NET 8 hosting bundle installed
- SQL Server accessible

### Steps

1. **Publish Application**
   ```bash
   dotnet publish -c Release -o publish
   ```

2. **Create Application Pool**
   - Name: PracticalAPI
   - .NET Version: No Managed Code
   - Pipeline Mode: Integrated

3. **Create Website**
   - Name: PracticalAPI
   - Physical Path: C:\inetpub\PracticalAPI\publish
   - Binding: https://yourdomain.com (port 443)
   - Application Pool: PracticalAPI

4. **Configure SSL Certificate**
   - Import certificate
   - Bind to HTTPS

5. **Set Environment Variables**
   In applicationhost.config or web.config:
   ```xml
   <system.webServer>
     <aspNetCore>
       <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
       <environmentVariable name="ConnectionStrings__DefaultConnection" value="..." />
       <environmentVariable name="Jwt__Key" value="..." />
     </aspNetCore>
   </system.webServer>
   ```

6. **Set File Permissions**
   ```bash
   icacls "C:\inetpub\PracticalAPI" /grant "IIS AppPool\PracticalAPI":(OI)(CI)F
   ```

---

## Deployment to Azure

### Prerequisites
- Azure subscription
- Azure CLI installed
- Visual Studio with Azure tools

### Steps

1. **Create Resources**
   ```bash
   # Create resource group
   az group create -n PracticalAPI -l eastus

   # Create App Service Plan
   az appservice plan create -g PracticalAPI -n PracticalAPIPlan --sku B1

   # Create Web App
   az webapp create -g PracticalAPI -n practicalapi-app -p PracticalAPIPlan

   # Create SQL Database
   az sql server create -g PracticalAPI -n practicalapi-sql -l eastus -u adminuser -p "Password123!"
   az sql db create -g PracticalAPI -s practicalapi-sql -n PracticalApiDb --sku Basic
   ```

2. **Configure Connection Strings**
   ```bash
   az webapp config connection-string set -g PracticalAPI -n practicalapi-app \
     -t SQLServer --settings DefaultConnection="Server=tcp:practicalapi-sql.database.windows.net..."
   ```

3. **Publish from Visual Studio**
   - Right-click project → Publish
   - Select Azure App Service
   - Follow wizard

4. **Apply Database Migrations**
   ```bash
   # Using Azure Cloud Shell or Kudu console
   dotnet ef database update --connection "Your-Connection-String"
   ```

---

## Docker Deployment

### Build Docker Image

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "PracticalBEsesi3.dll"]
```

### Build and Run

```bash
# Build image
docker build -t practicalapi:latest .

# Run container
docker run -d \
  -p 80:80 \
  -e ConnectionStrings__DefaultConnection="..." \
  -e Jwt__Key="..." \
  --name practicalapi \
  practicalapi:latest
```

---

## Post-Deployment Verification

### 1. Health Check
```bash
# Test API is running
curl -i https://yourdomain.com/swagger/index.html

# Should return 200 OK
```

### 2. Authentication Test
```bash
# Register
curl -X POST https://yourdomain.com/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test User",
    "email": "test@example.com",
    "password": "password123"
  }'

# Login
curl -X POST https://yourdomain.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "password123"
  }'

# Should return token
```

### 3. CRUD Operations Test
- Create task
- Read tasks
- Update task
- Delete task

### 4. Error Handling Test
- Invalid input
- Unauthorized access
- Not found (404)

---

## Monitoring & Logging

### Application Insights (Azure)
```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

### Serilog (Recommended)
```bash
dotnet add package Serilog
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
```

### Configure Logging
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

---

## Rollback Plan

### If Deployment Fails

```bash
# Step 1: Identify issue
# Check logs: Application Event Viewer, Application Insights

# Step 2: Rollback application
# Option 1: Restore previous publish folder
# Option 2: Redeploy previous version

# Step 3: Rollback database (if needed)
dotnet ef migrations remove
dotnet ef database update [previous-migration]

# Step 4: Verify system
# Run health checks
# Test critical functionality
```

---

## Security Checklist - Production

- [ ] HTTPS/TLS enabled
- [ ] JWT secret key is strong (random, 32+ characters)
- [ ] Database credentials not exposed in code
- [ ] Connection strings stored securely (Azure Key Vault, etc.)
- [ ] CORS origins are whitelisted (not *AllowAnyOrigin*)
- [ ] Admin credentials changed from defaults
- [ ] Firewall rules configured
- [ ] SQL Server port not publicly exposed
- [ ] Regular backups configured
- [ ] Error logging doesn't expose sensitive data

---

## Performance Tuning

### Database
```sql
-- Statistics update
UPDATE STATISTICS [PracticalApiDb];

-- Check index fragmentation
SELECT * FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED');
```

### Application
```csharp
// Connection pooling
"Server=.;Database=PracticalApiDb;Max Pool Size=100;"

// Query optimization
var tasks = await _context.TaskItems
    .AsNoTracking()  // Read-only queries
    .Where(t => t.UserId == userId)
    .Include(t => t.User)
    .ToListAsync();
```

---

## Maintenance Schedule

- [ ] Weekly: Monitor application logs and performance
- [ ] Monthly: Review security updates
- [ ] Monthly: Database backup verification
- [ ] Quarterly: Performance analysis and optimization
- [ ] Quarterly: Security audit
- [ ] Annually: Dependency updates

---

## Troubleshooting Common Issues

### Issue: 503 Service Unavailable
- Check application pool status
- Check event viewer for errors
- Check database connectivity

### Issue: 401 Unauthorized
- Verify JWT configuration
- Check token expiration
- Verify Authorization header format

### Issue: 500 Internal Server Error
- Check application logs
- Check database connection
- Verify configuration settings

### Issue: Slow Performance
- Check database indexes
- Profile slow queries
- Monitor application resources

---

## Documentation to Keep

- [ ] Migration scripts
- [ ] Deployment configuration
- [ ] Security certificates
- [ ] Connection strings (encrypted)
- [ ] API documentation
- [ ] User manuals

---

## Sign-Off

- [ ] Code review approved
- [ ] Testing completed
- [ ] Security review passed
- [ ] Performance acceptable
- [ ] Documentation complete
- [ ] Deployment plan reviewed

---

**Deployment Date:** _______________
**Deployed By:** _______________
**Reviewed By:** _______________
**Status:** Ready / Not Ready

---

For questions or issues, refer to:
- [README.md](README.md)
- [API_DOCUMENTATION.md](API_DOCUMENTATION.md)
- [CODE_ANALYSIS_REPORT.md](CODE_ANALYSIS_REPORT.md)
