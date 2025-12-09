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

## 👥 Demo Accounts

The solution comes with pre-seeded accounts:

| Email | Password | Role | Access |
|-------|----------|------|--------|
| admin@demo.local | P@ssword1! | Admin | Can view ALL tasks |
| alice@demo.local | P@ssword1! | User | Can only view her own tasks |
| bob@demo.local | P@ssword1! | User | Can only view his own tasks |

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
- Use `[Authorize]` attribute
- Implement role-based authorization
- Create custom authorization policies
- Enforce horizontal access control (row-level)
- Implement vertical access control (role-based)
- Prevent IDOR vulnerabilities

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

This is a teaching demo for the **Secure Code in C# & .NET** Udemy course.

**Remember:** This code contains intentional security vulnerabilities for educational purposes. Never use this directly in production without addressing all security gaps!

---

**Happy Learning! 🎓🔐**
