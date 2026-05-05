# Practical API CRUD - ASP.NET Core 8

A production-ready REST API for task management with JWT authentication, built with ASP.NET Core 8 and Entity Framework Core.

## 🚀 Features

- **User Authentication & Authorization**
  - JWT-based authentication
  - Secure password hashing with BCrypt
  - Role-based access control

- **Task Management**
  - Create, read, update, delete tasks
  - Task organization by user
  - Timestamp tracking (created, updated)

- **API Security**
  - Authorization on all protected endpoints
  - Input validation and sanitization
  - CORS protection with configurable whitelist
  - SQL injection prevention

- **Developer Experience**
  - Swagger/OpenAPI documentation
  - Comprehensive error messages
  - Request/Response DTOs
  - Entity Framework Core migrations

## 📋 Prerequisites

- .NET 8 SDK
- SQL Server (or LocalDB)
- Visual Studio 2022 (recommended) or VS Code
- Postman or similar API client for testing

## ⚙️ Installation

### 1. Clone the Repository
```bash
git clone https://github.com/waynh77/PracticalApiCrud.git
cd PracticalApiCrud
```

### 2. Configure Database Connection

Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=PracticalApiDb;Trusted_Connection=true;"
  }
}
```

### 3. Configure JWT Settings

Edit `appsettings.json`:
```json
{
  "Jwt": {
    "Key": "your-secret-key-minimum-32-characters-long",
    "Issuer": "PracticalAPI",
    "Audience": "PracticalAPIUsers",
    "ExpireMinutes": 60
  }
}
```

**Important:** Use a strong, random secret key in production!

### 4. Create Database

Open Package Manager Console and run:
```bash
dotnet ef database update
```

Or using Package Manager Console:
```
Update-Database
```

### 5. Run the Application

```bash
dotnet run
```

The API will be available at: `https://localhost:5001`

Swagger UI: `https://localhost:5001/swagger`

## 🧪 Quick Start - Testing with Postman

### 1. Register a New User
```
POST https://localhost:5001/api/auth/register
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "password123"
}
```

### 2. Login
```
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "password123"
}
```

Response will include a token. Copy it.

### 3. Create a Task
```
POST https://localhost:5001/api/taskitems
Content-Type: application/json
Authorization: Bearer {token}

{
  "judul": "Buy groceries",
  "deskripsi": "Milk, eggs, bread"
}
```

### 4. Get All Tasks
```
GET https://localhost:5001/api/taskitems
Authorization: Bearer {token}
```

### 5. Update a Task
```
PUT https://localhost:5001/api/taskitems/1
Content-Type: application/json
Authorization: Bearer {token}

{
  "title": "Buy groceries",
  "description": "Milk, eggs, bread, cheese",
  "isCompleted": true
}
```

### 6. Delete a Task
```
DELETE https://localhost:5001/api/taskitems/1
Authorization: Bearer {token}
```

## 📚 API Documentation

For complete API documentation, see [API_DOCUMENTATION.md](API_DOCUMENTATION.md)

### Main Endpoints

**Authentication:**
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login and get JWT token

**Tasks:**
- `GET /api/taskitems` - Get all user tasks
- `GET /api/taskitems/{id}` - Get task by ID
- `POST /api/taskitems` - Create new task
- `PUT /api/taskitems/{id}` - Update task
- `DELETE /api/taskitems/{id}` - Delete task

**Users:**
- `GET /api/users/{id}` - Get user profile
- `PUT /api/users/{id}` - Update user profile
- `DELETE /api/users/{id}` - Delete account

## 📊 Code Analysis

For detailed analysis of improvements and best practices, see [CODE_ANALYSIS_REPORT.md](CODE_ANALYSIS_REPORT.md)

### Key Improvements Made:
- ✅ Fixed security vulnerabilities
- ✅ Added comprehensive input validation
- ✅ Implemented proper error handling
- ✅ Added database indexes for performance
- ✅ Removed duplicate code
- ✅ Enhanced authorization checks
- ✅ Improved code quality and maintainability

## 🏗️ Project Structure

```
PracticalBEsesi3/
├── Controllers/
│   ├── AuthController.cs       # Authentication endpoints
│   ├── TaskItemsController.cs  # Task CRUD operations
│   └── UsersController.cs      # User management
├── Data/
│   └── AppDbContext.cs         # Database context
├── Dto/
│   └── Request/
│       ├── RegisterDto.cs      # Registration data
│       ├── LoginDto.cs         # Login data
│       ├── CreateTaskDto.cs    # Create task data
│       └── UpdateTaskDto.cs    # Update task data
├── Models/
│   ├── User.cs                 # User model
│   └── TaskItem.cs             # Task model
├── Migrations/                 # EF Core migrations
├── Program.cs                  # Application setup
└── appsettings.json            # Configuration
```

## 🔒 Security Best Practices

1. **JWT Token**
   - Tokens expire after configured time
   - Validate token on every protected request
   - Use HTTPS in production

2. **Password Security**
   - Passwords hashed with BCrypt
   - Never stored in plain text
   - Minimum 6 characters enforced

3. **Authorization**
   - Users can only access their own tasks
   - Endpoint authorization checks
   - Ownership verification

4. **Input Validation**
   - Server-side validation required
   - Email format validation
   - String length constraints
   - Required field validation

5. **CORS Protection**
   - Whitelist allowed origins
   - Credentials validation
   - Production uses specific domains

## 🐛 Common Issues & Solutions

### Issue: "Database Connection Failed"
**Solution:** Check connection string in `appsettings.json`
```bash
# Test connection
sqlcmd -S . -Q "select 1"
```

### Issue: "Invalid Token"
**Solution:** 
- Ensure JWT key is correct in both appsettings.json
- Check token hasn't expired
- Verify Bearer token format: `Authorization: Bearer {token}`

### Issue: "401 Unauthorized"
**Solution:**
- Register and login to get token
- Verify token in Authorization header
- Check token expiration

### Issue: "403 Forbidden"
**Solution:**
- Verify you own the resource
- Check if token belongs to correct user

## 📝 Environment Variables (Production)

For production, use environment variables instead of appsettings.json:

```bash
# Database
set ConnectionStrings__DefaultConnection="Server=prod-server;Database=ProdDb;..."

# JWT
set Jwt__Key="your-production-secret-key"
set Jwt__Issuer="YourCompany"
set Jwt__Audience="YourApp"
set Jwt__ExpireMinutes="120"
```

## 🚀 Deployment

### Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY bin/Release/net8.0/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "PracticalBEsesi3.dll"]
```

### Azure
1. Create App Service
2. Create SQL Database
3. Publish from Visual Studio
4. Configure connection strings in App Service settings

## 📊 Database Diagram

```
Users (1) -----(N) Tasks
  ├─ Id (PK)
  ├─ Name
  ├─ Email (Unique)
  ├─ PasswordHash
  └─ CreatedAt

TaskItems
  ├─ Id (PK)
  ├─ Title
  ├─ Description
  ├─ IsCompleted
  ├─ CreatedAt
  ├─ UpdatedAt
  └─ UserId (FK) → Users.Id
```

## 🧪 Unit Testing

Create test project:
```bash
dotnet new mstest -n PracticalBEsesi3.Tests
```

Add NuGet packages:
```bash
dotnet add package Moq
dotnet add package xunit
```

## 📞 Support & Contributing

- Issues: Open GitHub issue with detailed description
- Pull Requests: Welcome! Follow coding standards
- Documentation: Update docs with new features

## 📄 License

MIT License - feel free to use this project

## 🙏 Acknowledgments

- ASP.NET Core team for excellent framework
- Entity Framework Core for ORM
- JWT for authentication standard
- BCrypt for password hashing

---

**Last Updated:** May 2024
**Version:** 2.0 (Refactored)
**Status:** Production Ready ✅

For detailed API documentation, see [API_DOCUMENTATION.md](API_DOCUMENTATION.md)

For code improvements, see [CODE_ANALYSIS_REPORT.md](CODE_ANALYSIS_REPORT.md)
