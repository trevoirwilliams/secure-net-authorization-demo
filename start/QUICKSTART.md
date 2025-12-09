# 🚀 Quick Start Guide - SecureTaskHub

## Prerequisites

- .NET 10 SDK
- Docker Desktop (for Papercut SMTP - email testing)

## Get Up and Running in 5 Minutes

### Step 0: Setup Local Email Server (Papercut SMTP)

For password reset functionality, run Papercut SMTP in Docker:

```powershell
docker run -d --name papercut -p 25:25 -p 37408:37408 jijiechen/papercut:latest
```

Access Papercut Web UI at: http://localhost:37408

**See PAPERCUT-SETUP.md for detailed setup instructions.**

### Step 1: Build the Solution
```powershell
cd "c:\Users\Trevoir\OneDrive\Courses\Secure Coding in C# and .NET\authorization-demo\start"
dotnet build
```

### Step 2: Create the Database
```powershell
# The projects are configured to auto-apply migrations in Development mode
# Just run either project and the SQLite database will be created automatically

# OR manually run migrations:
dotnet ef database update --project SecureTaskHub.Infrastructure --startup-project SecureTaskHub.Api
```

### Step 3: Run the API
```powershell
cd SecureTaskHub.Api
dotnet run
```

Take note of the URL (e.g., `https://localhost:7xxx`)

### Step 4: Test the API

**Open Swagger UI:** Navigate to `https://localhost:7xxx/swagger`

**Get a JWT Token:**
1. Expand `POST /api/auth/login`
2. Click "Try it out"
3. Use these credentials:
```json
{
  "email": "admin@demo.local",
  "password": "AdminPassword123!"
}
```
4. Click "Execute"
5. Copy the `token` from the response

**Authorize Swagger:**
1. Click the "Authorize" button at the top
2. Enter: `Bearer <your-token-here>` (include the word "Bearer" and a space)
3. Click "Authorize"
4. Click "Close"

**Test Endpoints:**
- `GET /api/tasks` - Get your tasks (returns admin's tasks)
- `GET /api/tasks/all` - **INSECURE!** Returns ALL tasks (the vulnerability!)
- `GET /api/tasks/admin/all` - Properly secured admin endpoint

### Step 5: Run the Web App

Open a new terminal:
```powershell
cd "c:\Users\Trevoir\OneDrive\Courses\Secure Coding in C# and .NET\authorization-demo\start\SecureTaskHub.Web"
dotnet run
```

Navigate to `https://localhost:7xxx` (check console output for exact URL)

**Log in with:**
- **Admin:** admin@demo.local / Admin123!
- **Alice:** alice@demo.local / Alice123!
- **Bob:** bob@demo.local / Bob123!

**Test Password Reset:**
1. Click "Forgot your password?" on login page
2. Enter user email (e.g., admin@demo.local)
3. Check Papercut UI (http://localhost:37408) for the reset email
4. Click the link in the email to reset the password

### Step 6: Explore Security Features

#### Test Horizontal Access Control
1. Log in as **Alice**
2. Go to "My Tasks" - should see only Alice's tasks (3 tasks)
3. Log out
4. Log in as **Bob**
5. Go to "My Tasks" - should see only Bob's tasks (3 tasks)

#### Test Vertical Access Control
1. Log in as **Bob** (regular user)
2. Try to access Admin Dashboard
3. You should be redirected (no access)
4. Log out
5. Log in as **Admin**
6. Access Admin Dashboard - should see ALL 8 tasks

#### Exploit the Insecure API Endpoint
1. Log in as **Alice** via the API (use Swagger)
2. Call `GET /api/tasks/all`
3. **BUG:** Alice can see everyone's tasks including Bob's and Admin's!
4. This demonstrates broken access control

##  Demo Accounts Summary

| User | Email | Password | Role | Tasks |
|------|-------|----------|------|-------|
| Admin | admin@demo.local | Admin123! | Admin | 2 tasks |
| Alice | alice@demo.local | Alice123! | User | 3 tasks |
| Bob | bob@demo.local | Bob123! | User | 3 tasks |

**Note:** If passwords don't work, use the password reset feature!

## 📝 What's Next?

1. Setup Papercut SMTP (see PAPERCUT-SETUP.md) for password reset testing
2. Review the `SECURITY-GAPS.md` file for all intentional vulnerabilities
3. Follow the course modules to fix each security gap
4. Test each fix to verify proper security implementation
5. Compare before/after implementations

## 🔍 Key Files to Examine

### Authentication & Authorization Setup
- `SecureTaskHub.Api/Program.cs` - JWT configuration
- `SecureTaskHub.Web/Program.cs` - Cookie authentication
- `SecureTaskHub.Api/Controllers/AuthController.cs` - Token generation

### Access Control Implementation
- `SecureTaskHub.Api/Controllers/TasksController.cs` - API endpoints
- `SecureTaskHub.Infrastructure/Repositories/TaskItemRepository.cs` - Data filtering
- `SecureTaskHub.Web/Pages/AdminDashboard.cshtml.cs` - Role-based page access

### Data & Seeding
- `SecureTaskHub.Infrastructure/Data/ApplicationDbContext.cs` - Users, roles, and tasks

## 🛠️ Troubleshooting

### Database Connection Issues
If you get database errors:
```powershell
# Drop and recreate the database
dotnet ef database drop --project SecureTaskHub.Infrastructure --startup-project SecureTaskHub.Api --force
dotnet ef database update --project SecureTaskHub.Infrastructure --startup-project SecureTaskHub.Api
```

### Port Conflicts
If ports are in use, edit `launchSettings.json` in Properties folder of each project.

### Package Restore Issues
```powershell
dotnet clean
dotnet restore
dotnet build
```

## 📚 Learning Path

1. **Module 1:** Understand the architecture and authentication setup
2. **Module 2:** Identify the 13+ security gaps
3. **Module 3:** Fix each vulnerability one by one
4. **Module 4:** Implement advanced features (MFA, refresh tokens, etc.)
5. **Module 5:** Deploy securely to production

## ⚠️ Remember

This is a **TEACHING DEMO** with **INTENTIONAL SECURITY VULNERABILITIES**.

**Never use this code in production without fixing ALL security gaps!**

All gaps are marked with `// TODO Secure:` comments throughout the codebase.

---

**Happy Learning! 🎓**

For detailed documentation, see `README.md`  
For security gap details, see `SECURITY-GAPS.md`
