# Security Gaps Reference - For Teaching

This document lists all intentional security vulnerabilities in the SecureTaskHub demo, designed for teaching secure coding practices.

## 🎯 How to Use This Guide

Each section contains:
- **Location:** Where to find the code
- **Current State:** What's wrong
- **Security Risk:** Why it's a problem
- **Fix:** How to make it secure

---

## 1. Weak Password Policy

### Location
- `SecureTaskHub.Api/Program.cs` (lines ~35-40)
- `SecureTaskHub.Web/Program.cs` (lines ~26-31)

### Current State
```csharp
options.Password.RequireDigit = false;
options.Password.RequireLowercase = false;
options.Password.RequireUppercase = false;
options.Password.RequireNonAlphanumeric = false;
options.Password.RequiredLength = 6;
```

### Security Risk
- Allows weak passwords like "123456" or "password"
- Easy to guess and brute force
- OWASP A07:2021 - Identification and Authentication Failures

### Fix
```csharp
options.Password.RequireDigit = true;
options.Password.RequireLowercase = true;
options.Password.RequireUppercase = true;
options.Password.RequireNonAlphanumeric = true;
options.Password.RequiredLength = 12; // NIST recommends 8-12+
options.Password.RequiredUniqueChars = 4;
```

---

## 2. Account Lockout Disabled

### Location
- `SecureTaskHub.Api/Program.cs` (line ~43)
- `SecureTaskHub.Web/Program.cs` (line ~34)

### Current State
```csharp
options.Lockout.AllowedForNewUsers = false;
```

Also in `AuthController.cs`:
```csharp
await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
```

### Security Risk
- No protection against brute force attacks
- Attackers can try unlimited password combinations
- OWASP A07:2021 - Identification and Authentication Failures

### Fix
```csharp
options.Lockout.AllowedForNewUsers = true;
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
options.Lockout.MaxFailedAccessAttempts = 5;
```

And in AuthController:
```csharp
await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
```

---

## 3. Insecure API Endpoint - Broken Access Control

### Location
- `SecureTaskHub.Api/Controllers/TasksController.cs` (lines ~58-70)

### Current State
```csharp
[HttpGet("all")]
public async Task<IActionResult> GetAllTasks()
{
    // SECURITY GAP: No authorization check!
    var tasks = await _repository.GetAllTasksAsync();
    return Ok(tasks);
}
```

### Security Risk
- **ANY** authenticated user can see **ALL** tasks from **ALL** users
- Violates horizontal access control (Broken Access Control)
- OWASP A01:2021 - Broken Access Control
- Classic IDOR (Insecure Direct Object Reference) vulnerability

### Fix
**Option 1:** Remove the endpoint entirely

**Option 2:** Restrict to Admin only
```csharp
[HttpGet("all")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> GetAllTasks()
{
    var tasks = await _repository.GetAllTasksAsync();
    return Ok(tasks);
}
```

---

## 4. JWT Token - Too Long Expiration

### Location
- `SecureTaskHub.Api/Controllers/AuthController.cs` (line ~109)

### Current State
```csharp
expires: DateTime.UtcNow.AddHours(24), // Too long!
```

### Security Risk
- 24-hour tokens increase attack window
- If token is stolen, attacker has 24 hours of access
- Violates principle of least privilege (time-wise)

### Fix
```csharp
expires: DateTime.UtcNow.AddMinutes(15), // Short-lived access token
// Implement refresh tokens for longer sessions
```

---

## 5. Missing Refresh Token Implementation

### Location
- `SecureTaskHub.Api/Controllers/AuthController.cs`

### Current State
No refresh token mechanism exists.

### Security Risk
- Forces use of long-lived access tokens
- No way to revoke access without changing secrets
- Cannot implement "remember me" securely

### Fix
Implement refresh token flow:
1. Issue short-lived access token (15 minutes)
2. Issue long-lived refresh token (7-30 days)
3. Store refresh tokens securely (database)
4. Add `/api/auth/refresh` endpoint
5. Implement token rotation and revocation

---

## 6. Weak JWT Secret in Configuration

### Location
- `SecureTaskHub.Api/Program.cs` (line ~77)
- `SecureTaskHub.Api/appsettings.json`

### Current State
```csharp
var secretKey = _configuration["Jwt:Secret"] ?? "ThisIsATemporarySecretKeyForDevelopmentOnly123!";
```

### Security Risk
- Secret stored in plaintext in appsettings
- Source control exposure risk
- Not cryptographically strong
- Shared across environments

### Fix
**Development:**
Use User Secrets:
```powershell
dotnet user-secrets set "Jwt:Secret" "<strong-random-secret>"
```

**Production:**
Use Azure Key Vault or environment variables:
```csharp
var secretKey = builder.Configuration["KeyVault:JwtSecret"];
```

Generate strong secret:
```csharp
// Use at least 256 bits (32 bytes)
var randomBytes = new byte[32];
RandomNumberGenerator.Fill(randomBytes);
var secret = Convert.ToBase64String(randomBytes);
```

---

## 7. Admin Page - Missing Authorization Attribute

### Location
- `SecureTaskHub.Web/Pages/AdminDashboard.cshtml.cs` (class declaration)

### Current State
```csharp
public class AdminDashboardModel : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        IsAdmin = User.IsInRole("Admin");
        if (!IsAdmin)
        {
            return RedirectToPage("/MyTasks");
        }
        // ...
    }
}
```

### Security Risk
- Authorization check happens AFTER page is accessed
- Relies on code-level check only (no declarative security)
- Bypass possible if code is modified
- Not defense-in-depth

### Fix
```csharp
[Authorize(Roles = "Admin")] // Add this attribute!
public class AdminDashboardModel : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        var tasks = await _repository.GetAllTasksAsync();
        AllTasks = tasks.ToList();
        return Page();
    }
}
```

Or use page conventions in Program.cs:
```csharp
options.Conventions.AuthorizePage("/AdminDashboard", "RequireAdminRole");
```

---

## 8. CORS Allow Any Origin

### Location
- `SecureTaskHub.Api/Program.cs` (lines ~149-156)

### Current State
```csharp
policy.AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader();
```

### Security Risk
- Any website can make requests to your API
- Cross-Origin attacks possible
- CSRF potential (though JWT helps)

### Fix
```csharp
policy.WithOrigins(
        "https://yourdomain.com",
        "https://localhost:5001"
    )
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials(); // Only if using cookies
```

---

## 9. No Multi-Factor Authentication (MFA)

### Location
- Entire solution

### Current State
Only username/password authentication is implemented.

### Security Risk
- Compromised passwords lead to immediate account takeover
- No defense against phishing
- No compliance with security standards (NIST, PCI-DSS require MFA)

### Fix
1. Enable TOTP (Time-based One-Time Password):
```csharp
options.SignIn.RequireConfirmedAccount = true;
// Implement QR code generation for authenticator apps
```

2. Add SMS verification:
```csharp
services.AddTransient<ISmsSender, SmsSender>();
```

3. Implement backup codes for account recovery

---

## 10. Ownership Transfer Vulnerability

### Location
- `SecureTaskHub.Infrastructure/Repositories/TaskItemRepository.cs` (line ~86)

### Current State
```csharp
public async Task<bool> UpdateTaskAsync(TaskItem task, string userId, bool isAdmin)
{
    // ...
    existingTask.OwnerUserId = task.OwnerUserId; // SECURITY GAP!
}
```

### Security Risk
- User could change `OwnerUserId` in update request
- Task ownership can be transferred without authorization
- Potential data theft or privilege escalation

### Fix
```csharp
public async Task<bool> UpdateTaskAsync(TaskItem task, string userId, bool isAdmin)
{
    // ...
    existingTask.Title = task.Title;
    existingTask.Description = task.Description;
    existingTask.Status = task.Status;
    
    // Don't allow OwnerUserId changes unless admin explicitly transfers
    if (isAdmin && task.OwnerUserId != existingTask.OwnerUserId)
    {
        _logger.LogWarning("Admin {AdminId} transferring task {TaskId} from {OldOwner} to {NewOwner}",
            userId, task.Id, existingTask.OwnerUserId, task.OwnerUserId);
        existingTask.OwnerUserId = task.OwnerUserId;
    }
    // Otherwise, keep existing OwnerUserId
}
```

---

## 11. No Email Confirmation

### Location
- `SecureTaskHub.Api/Program.cs` (line ~47)
- `SecureTaskHub.Web/Program.cs` (line ~38)

### Current State
```csharp
options.SignIn.RequireConfirmedEmail = false;
```

### Security Risk
- Users can register with fake/others' email addresses
- No verification of identity
- Potential for account takeover

### Fix
```csharp
options.SignIn.RequireConfirmedEmail = true;

// Implement email sending
services.AddTransient<IEmailSender, EmailSender>();

// Add confirmation endpoint in AuthController
[HttpPost("confirm-email")]
public async Task<IActionResult> ConfirmEmail(string userId, string token)
{
    var user = await _userManager.FindByIdAsync(userId);
    var result = await _userManager.ConfirmEmailAsync(user, token);
    // ...
}
```

---

## 12. Missing Input Validation

### Location
- Various controllers and models

### Current State
Minimal validation:
```csharp
if (string.IsNullOrWhiteSpace(request.Title))
{
    return BadRequest(new { message = "Title is required" });
}
```

### Security Risk
- SQL injection (mitigated by EF Core parameterization)
- XSS attacks
- Data integrity issues
- Denial of Service (huge strings)

### Fix
Use FluentValidation:
```csharp
public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title too long")
            .Must(NotContainHtml).WithMessage("HTML not allowed");
            
        RuleFor(x => x.Description)
            .MaximumLength(2000);
    }
}
```

---

## 13. No Rate Limiting

### Location
- API project (missing)

### Current State
No rate limiting implemented.

### Security Risk
- Brute force attacks on login
- API abuse
- Denial of Service
- Resource exhaustion

### Fix
Use ASP.NET Core Rate Limiting:
```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromMinutes(1);
    });
});

// Apply to login endpoint
[EnableRateLimiting("fixed")]
[HttpPost("login")]
public async Task<IActionResult> Login(...)
```

---

## 📊 Priority Order for Fixes

1. **CRITICAL** - Fix Insecure Endpoint (#3)
2. **HIGH** - Enable Account Lockout (#2)
3. **HIGH** - Add Authorization Attribute to Admin Page (#7)
4. **HIGH** - Fix Ownership Transfer (#10)
5. **MEDIUM** - Strengthen Password Policy (#1)
6. **MEDIUM** - Reduce Token Expiration (#4)
7. **MEDIUM** - Secure JWT Secret (#6)
8. **MEDIUM** - Fix CORS (#8)
9. **LOW** - Implement Refresh Tokens (#5)
10. **LOW** - Add MFA (#9)
11. **LOW** - Require Email Confirmation (#11)
12. **LOW** - Add Input Validation (#12)
13. **LOW** - Implement Rate Limiting (#13)

---

## 🧪 Testing Each Vulnerability

See README.md "Testing Scenarios" section for step-by-step instructions on demonstrating each vulnerability.

---

**Remember:** These are TEACHING vulnerabilities. In real production code, ALL of these should be fixed!
