using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SecureTaskHub.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SecureTaskHub.Api.Controllers;

/// <summary>
/// Handles authentication operations including login and token generation.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // TODO Secure: Add input validation and rate limiting to prevent brute force attacks
        // TODO Secure: Implement account lockout after multiple failed attempts
        
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            // TODO Secure: Don't reveal whether user exists or not (timing attack mitigation)
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        
        // TODO Secure: Enable lockoutOnFailure in production
        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed login attempt for user {Email}", request.Email);
            return Unauthorized(new { message = "Invalid credentials" });
        }

        // Generate JWT token
        var token = await GenerateJwtToken(user);
        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new LoginResponse
        {
            Token = token,
            Email = user.Email!,
            UserId = user.Id,
            Roles = roles.ToList()
        });
    }

    private async Task<string> GenerateJwtToken(IdentityUser user)
    {
        // TODO Secure: Store JWT secret in secure configuration (Azure Key Vault, User Secrets)
        // TODO Secure: Use stronger, randomly generated secrets (256+ bits)
        var secretKey = _configuration["Jwt:Secret"] ?? "ThisIsATemporarySecretKeyForDevelopmentOnly123!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // TODO Secure: Reduce token expiration time (currently 24 hours is too long)
        // TODO Secure: Implement refresh tokens for better security
        // TODO Secure: Add proper Issuer and Audience validation
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "SecureTaskHub",
            audience: _configuration["Jwt:Audience"] ?? "SecureTaskHub",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24), // Too long!
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Gets information about the current authenticated user.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            userId = user.Id,
            email = user.Email,
            roles = roles
        });
    }

    // TODO Secure: Add registration endpoint with proper validation
    // TODO Secure: Add password reset functionality
    // TODO Secure: Add email confirmation workflow
    // TODO Secure: Add MFA enrollment and verification endpoints
}
