# API Documentation - Practical CRUD

## Overview
RESTful API untuk manajemen task dengan autentikasi JWT dan database SQL Server.

## Base URL
```
https://localhost:5001/api
```

---

## 🔐 Authentication Endpoints

### 1. Register User
**POST** `/auth/register`

**Request Body:**
```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "password123"
}
```

**Response (201 Created):**
```json
{
  "message": "Register berhasil"
}
```

**Validation Rules:**
- Nama: 3-100 karakter (required)
- Email: Format valid, unique (required)
- Password: Minimal 6 karakter (required)

---

### 2. Login
**POST** `/auth/login`

**Request Body:**
```json
{
  "email": "john@example.com",
  "password": "password123"
}
```

**Response (200 OK):**
```json
{
  "message": "Login berhasil",
  "user": {
    "id": 1,
    "name": "John Doe",
    "email": "john@example.com"
  },
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Token Usage:**
```
Authorization: Bearer {token}
```

---

## 📋 Task Endpoints

### 1. Get All My Tasks
**GET** `/taskitems`
**Auth:** Required (Bearer Token)

**Response (200 OK):**
```json
{
  "count": 2,
  "data": [
    {
      "id": 1,
      "title": "Buy groceries",
      "description": "Milk, eggs, bread",
      "isCompleted": false,
      "createdAt": "2024-05-15T10:30:00Z",
      "updatedAt": "2024-05-15T10:30:00Z",
      "user": {
        "id": 1,
        "name": "John Doe"
      }
    }
  ]
}
```

---

### 2. Get Task by ID
**GET** `/taskitems/{id}`
**Auth:** Required

**Response (200 OK):**
```json
{
  "id": 1,
  "title": "Buy groceries",
  "description": "Milk, eggs, bread",
  "isCompleted": false,
  "createdAt": "2024-05-15T10:30:00Z",
  "updatedAt": "2024-05-15T10:30:00Z",
  "user": {
    "id": 1,
    "name": "John Doe"
  }
}
```

---

### 3. Create Task
**POST** `/taskitems`
**Auth:** Required

**Request Body:**
```json
{
  "judul": "Buy groceries",
  "deskripsi": "Milk, eggs, bread"
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "title": "Buy groceries",
  "description": "Milk, eggs, bread",
  "isCompleted": false,
  "createdAt": "2024-05-15T10:30:00Z",
  "updatedAt": "2024-05-15T10:30:00Z"
}
```

**Validation Rules:**
- Judul: 3-200 karakter (required)
- Deskripsi: Maksimal 1000 karakter (optional)

---

### 4. Update Task
**PUT** `/taskitems/{id}`
**Auth:** Required

**Request Body:**
```json
{
  "title": "Buy groceries",
  "description": "Milk, eggs, bread, cheese",
  "isCompleted": true
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "title": "Buy groceries",
  "description": "Milk, eggs, bread, cheese",
  "isCompleted": true,
  "createdAt": "2024-05-15T10:30:00Z",
  "updatedAt": "2024-05-15T11:45:00Z"
}
```

**Validation Rules:**
- Title: 3-200 karakter (required)
- Description: Maksimal 1000 karakter (optional)

---

### 5. Delete Task
**DELETE** `/taskitems/{id}`
**Auth:** Required

**Response (204 No Content)**

---

## 👤 User Endpoints

### 1. Get User Profile
**GET** `/users/{id}`
**Auth:** Required (Owner only)

**Response (200 OK):**
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com",
  "taskCount": 5
}
```

---

### 2. Update User Profile
**PUT** `/users/{id}`
**Auth:** Required (Owner only)

**Request Body:**
```json
{
  "name": "John Doe Jr."
}
```

**Response (200 OK):**
```json
{
  "message": "Profil berhasil diperbarui",
  "id": 1,
  "name": "John Doe Jr.",
  "email": "john@example.com"
}
```

---

### 3. Delete User Account
**DELETE** `/users/{id}`
**Auth:** Required (Owner only)

**Response (200 OK):**
```json
{
  "message": "Akun berhasil dihapus"
}
```

---

## 🔍 Error Responses

### 400 Bad Request
```json
{
  "message": "Validation error description",
  "errors": {
    "field": ["error message"]
  }
}
```

### 401 Unauthorized
```json
{
  "message": "Email atau password salah"
}
```

### 403 Forbidden
```json
{
  "message": "Access denied"
}
```

### 404 Not Found
```json
{
  "message": "Task tidak ditemukan"
}
```

### 500 Internal Server Error
```json
{
  "message": "Terjadi kesalahan saat register",
  "error": "error details"
}
```

---

## 📝 Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=PracticalApiDb;Trusted_Connection=true;"
  },
  "Jwt": {
    "Key": "your-secret-key-here-minimum-32-characters-long",
    "Issuer": "YourAppName",
    "Audience": "YourAppUsers",
    "ExpireMinutes": 60
  },
  "AllowedOrigins": [
    "http://localhost:3000",
    "http://localhost:3001"
  ]
}
```

---

## 🔒 Security Features

1. **JWT Authentication** - Secure token-based authentication
2. **Password Hashing** - BCrypt password hashing
3. **CORS Protection** - Configurable allowed origins
4. **Authorization** - User can only access/modify own data
5. **Input Validation** - Server-side validation for all inputs
6. **SQL Injection Prevention** - Entity Framework parameterized queries

---

## 📊 Database Schema

### Users Table
- `Id` (PK, int)
- `Name` (string, 100)
- `Email` (string, 255, unique)
- `PasswordHash` (string)
- `CreatedAt` (datetime)

### TaskItems Table
- `Id` (PK, int)
- `Title` (string, 200)
- `Description` (string, 1000)
- `IsCompleted` (bool)
- `CreatedAt` (datetime)
- `UpdatedAt` (datetime)
- `UserId` (FK, int) → Users.Id (Cascade Delete)

---

## 🚀 Getting Started

1. Clone repository
2. Update connection string in `appsettings.json`
3. Set JWT configuration in `appsettings.json`
4. Run migrations: `dotnet ef database update`
5. Start application: `dotnet run`
6. Access Swagger: `https://localhost:5001/swagger`
