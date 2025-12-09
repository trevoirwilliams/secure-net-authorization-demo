# SecureTaskHub - Teaching Demo for Secure Coding in C# & .NET

A comprehensive teaching solution demonstrating **secure authentication and authorization** in ASP.NET Core 10, designed for a Udemy course on secure coding practices.

## 🎯 Purpose

This solution is intentionally built with **both secure implementations and deliberate security gaps** (clearly marked) to teach:

- ✅ **Authentication vs Authorization** - Understanding the difference
- ✅ **ASP.NET Identity** - User management, password hashing, roles
- ✅ **JWT Bearer Authentication** - Token-based auth for APIs
- ✅ **Cookie Authentication** - Session-based auth for web apps
- ✅ **Horizontal Access Control** - Row-level security (users see only their own data)
- ✅ **Vertical Access Control** - Role-based authorization (admin vs user)
- ⚠️ **Common Security Mistakes** - And how to fix them

## 🏗️ Architecture

### Project Structure

```
SecureTaskHub/
├── SecureTaskHub.Domain/          # Core entities and interfaces
│   ├── Entities/
│   │   └── TaskItem.cs           # Main entity with OwnerUserId
│   ├── Enums/
│   │   └── TaskStatus.cs         # Task status enum
│   └── Interfaces/
│       └── ITaskItemRepository.cs
│
├── SecureTaskHub.Infrastructure/  # Data access and Identity
│   ├── Data/
│   │   └── ApplicationDbContext.cs  # EF Core + Identity context
│   └── Repositories/
│       └── TaskItemRepository.cs    # Repository with access control
│
├── SecureTaskHub.Api/             # Web API with JWT authentication
│   ├── Controllers/
│   │   ├── AuthController.cs     # Login, token generation
│   │   └── TasksController.cs    # Task CRUD with auth checks
│   ├── Models/
│   │   └── ApiModels.cs          # DTOs
│   └── Program.cs                # JWT + Identity configuration
│
└── SecureTaskHub.Web/             # Razor Pages with cookie auth
    ├── Pages/
    │   ├── Index.cshtml           # Welcome page
    │   ├── MyTasks.cshtml         # User's own tasks
    │   └── AdminDashboard.cshtml  # Admin-only view
    └── Program.cs                 # Cookie auth + Identity configuration
```

## 👥 Demo Accounts

The solution comes with pre-seeded accounts:

| Email | Password | Role | Access |
|-------|----------|------|--------|
| admin@demo.local | AdminPassword123! | Admin | Can view ALL tasks |
| alice@demo.local | AlicePassword123! | User | Can only view her own tasks |
| bob@demo.local | BobPassword123! | User | Can only view his own tasks |

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK
- SQLite (no installation needed)
- Visual Studio 2022 or VS Code
- Entity Framework Core Tools

### Setup Instructions

#### 1. Clone and Restore

```powershell
cd "c:\Users\Trevoir\OneDrive\Courses\Secure Coding in C# and .NET\authorization-demo\start"
dotnet restore
```

#### 2. Create Database Migration

From the solution root directory:

```powershell
dotnet ef migrations add InitialCreate --project SecureTaskHub.Infrastructure --startup-project SecureTaskHub.Api
```

#### 3. Apply Migration

```powershell
dotnet ef database update --project SecureTaskHub.Infrastructure --startup-project SecureTaskHub.Api
```

Alternatively, both projects are configured to auto-apply migrations in Development mode.

#### 4. Run the Applications

**Run API:**
```powershell
cd SecureTaskHub.Api
dotnet run
```
API will be available at: `https://localhost:7xxx` (check console output)

**Run Web App:**
```powershell
cd SecureTaskHub.Web
dotnet run
```
Web will be available at: `https://localhost:7xxx` (check console output)

## 🔐 Security Features Demonstrated

### ✅ Properly Implemented

1. **ASP.NET Identity Integration**
   - User storage with hashed passwords
   - Role management (Admin, User)
   - Claims-based authentication

2. **JWT Authentication (API)**
   - Token generation with claims
   - Bearer token validation
   - Role claims in tokens

3. **Cookie Authentication (Web)**
   - Secure cookie configuration
   - HttpOnly and Secure flags
   - Sliding expiration

4. **Horizontal Access Control**
   - Repository filters by OwnerUserId
   - Users cannot access others' tasks
   - Proper authorization checks in endpoints

5. **Vertical Access Control**
   - Role-based authorization (`[Authorize(Roles = "Admin")]`)
   - Admin-only endpoints and pages
   - Policy-based authorization setup

### ⚠️ Intentional Security Gaps (Teaching Points)

All gaps are marked with `// TODO Secure:` comments:

#### 1. **Weak Password Policy**
```csharp
// TODO Secure: Strengthen password requirements for production
options.Password.RequireDigit = false; // Should be true
options.Password.RequiredLength = 6;    // Should be 12+
```

#### 2. **Lockout Disabled**
```csharp
// TODO Secure: Enable account lockout to prevent brute force attacks
options.Lockout.AllowedForNewUsers = false; // Should be true!
```

#### 3. **Insecure API Endpoint** (Deliberate Vulnerability)
```csharp
// GET /api/tasks/all - Returns ALL tasks without authorization
// This violates horizontal access control!
```

#### 4. **Long Token Expiration**
```csharp
// TODO Secure: Reduce token expiration time (currently 24 hours is too long)
expires: DateTime.UtcNow.AddHours(24)
```

#### 5. **No Refresh Tokens**
```csharp
// TODO Secure: Implement refresh tokens for better security
```

#### 6. **Weak JWT Secret**
```csharp
// TODO Secure: Store JWT secret in Azure Key Vault or User Secrets
var secretKey = "ThisIsATemporarySecretKeyForDevelopmentOnly123!";
```

#### 7. **Admin Page with Code-Only Check**
```csharp
// AdminDashboard.cshtml.cs
// TODO Secure: Add [Authorize(Roles = "Admin")] attribute
// Currently only checks role in code, not via middleware
```

#### 8. **CORS Allow All Origins**
```csharp
// TODO Secure: Restrict CORS to specific origins in production
policy.AllowAnyOrigin()
```

#### 9. **No MFA/2FA**
```csharp
// TODO Secure: Add MFA enrollment and verification endpoints
```

#### 10. **Ownership Transfer Vulnerability**
```csharp
// TaskItemRepository.UpdateTaskAsync
existingTask.OwnerUserId = task.OwnerUserId; // SECURITY GAP!
```

## 📚 Teaching Topics

### Authentication vs Authorization

- **Authentication (WHO):** Verifying user identity
  - Login credentials
  - JWT tokens
  - Identity cookies
  
- **Authorization (WHAT):** Determining what user can do
  - `[Authorize]` attribute
  - Role-based policies
  - Custom authorization handlers

### Access Control Types

**Horizontal Access Control (Row-Level)**
- Users see only their own data
- Filter by `OwnerUserId`
- Example: Alice cannot see Bob's tasks

**Vertical Access Control (Role-Based)**
- Different permissions for different roles
- Admin can see everything
- Regular users have limited access

## 🧪 Testing Scenarios

### Scenario 1: Test Horizontal Access Control
1. Log in as **Alice** (alice@demo.local)
2. Navigate to "My Tasks" - should see only Alice's tasks
3. Try to access Bob's task directly via API:
   ```
   GET /api/tasks/6 (Bob's task)
   ```
   Should return 404 (not found/unauthorized)

### Scenario 2: Test Vertical Access Control
1. Log in as **Bob** (bob@demo.local)
2. Try to access Admin Dashboard - should be redirected
3. Try to call admin endpoint:
   ```
   GET /api/tasks/admin/all
   ```
   Should return 403 Forbidden

### Scenario 3: Exploit Insecure Endpoint
1. Log in as **Alice**
2. Call the deliberately insecure endpoint:
   ```
   GET /api/tasks/all
   ```
   **BUG:** Alice can see ALL tasks including Bob's and Admin's!
3. **Fix:** Remove this endpoint or add `[Authorize(Roles = "Admin")]`

### Scenario 4: Test API with Swagger
1. Run API project
2. Open Swagger UI: `https://localhost:7xxx/swagger`
3. Click "Authorize" and get a JWT token via `/api/auth/login`
4. Test authenticated endpoints

## 🔧 Common Operations

### View Database

```powershell
# List migrations
dotnet ef migrations list --project SecureTaskHub.Infrastructure --startup-project SecureTaskHub.Api

# View connection string
# Check appsettings.json in API or Web projects
```

### Reset Database

```powershell
dotnet ef database drop --project SecureTaskHub.Infrastructure --startup-project SecureTaskHub.Api
dotnet ef database update --project SecureTaskHub.Infrastructure --startup-project SecureTaskHub.Api
```

### Add New Migration

```powershell
dotnet ef migrations add <MigrationName> --project SecureTaskHub.Infrastructure --startup-project SecureTaskHub.Api
```

## 📖 Course Flow

### Module 1: Authentication Basics
- Understand password hashing with ASP.NET Identity
- Implement JWT token generation
- Configure cookie authentication

### Module 2: Authorization Fundamentals
- Use `[Authorize]` attribute
- Implement role-based authorization
- Create custom authorization policies

### Module 3: Access Control
- Enforce horizontal access control (row-level)
- Implement vertical access control (role-based)
- Prevent IDOR vulnerabilities

### Module 4: Fixing Security Gaps
- Strengthen password policies
- Enable account lockout
- Implement MFA/2FA
- Use secure token storage
- Add refresh tokens
- Implement proper CORS

## ⚡ Quick Reference

### Important Files

- `Program.cs` (API) - JWT configuration and middleware setup
- `Program.cs` (Web) - Cookie authentication and Identity setup
- `ApplicationDbContext.cs` - Database seeding with demo users
- `TasksController.cs` - API endpoints with auth examples
- `AdminDashboard.cshtml.cs` - Page with intentional security gap

### Key Concepts

```csharp
// Authentication: WHO is the user?
app.UseAuthentication();

// Authorization: WHAT can the user do?
app.UseAuthorization();

// Horizontal: Filter by user
var tasks = await _repository.GetTasksByUserIdAsync(userId);

// Vertical: Check role
[Authorize(Roles = "Admin")]
```

## 🛡️ Production Checklist

Before deploying to production, fix all `// TODO Secure:` items:

- [ ] Strengthen password requirements
- [ ] Enable account lockout
- [ ] Implement email confirmation
- [ ] Add MFA/2FA
- [ ] Use secure JWT secret storage (Azure Key Vault)
- [ ] Reduce token expiration time
- [ ] Implement refresh tokens
- [ ] Remove insecure endpoints
- [ ] Add proper authorization attributes
- [ ] Configure CORS for specific origins
- [ ] Enable HTTPS everywhere
- [ ] Add logging for security events
- [ ] Implement rate limiting
- [ ] Add input validation

## 📞 Support

This is a teaching demo for the **Secure Code in C# & .NET** Udemy course.

**Remember:** This code contains intentional security vulnerabilities for educational purposes. Never use this directly in production without addressing all security gaps!

---

**Happy Learning! 🎓🔐**
