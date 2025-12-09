using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureTaskHub.Domain.Entities;
using SecureTaskHub.Domain.Interfaces;

namespace SecureTaskHub.Web.Pages;

/// <summary>
/// Admin dashboard that shows ALL tasks in the system.
/// Demonstrates vertical access control (role-based authorization).
/// 
/// SECURITY NOTE: This page has intentional gaps for teaching purposes.
/// </summary>
public class AdminDashboardModel : PageModel
{
    private readonly ITaskItemRepository _repository;
    private readonly ILogger<AdminDashboardModel> _logger;

    public AdminDashboardModel(ITaskItemRepository repository, ILogger<AdminDashboardModel> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public List<TaskItem> AllTasks { get; set; } = new();
    public bool IsAdmin { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // TODO Secure: This check is done in the page code, but should ALSO be done via
        // [Authorize(Roles = "Admin")] attribute or page conventions for defense in depth
        
        IsAdmin = User.IsInRole("Admin");
        
        // INTENTIONAL SECURITY GAP: This only checks role membership in code
        // If someone bypasses the UI check, they could still access the data
        if (!IsAdmin)
        {
            // This is CLIENT-SIDE protection - not enough!
            // TODO Secure: Add [Authorize(Roles = "Admin")] attribute above class
            _logger.LogWarning("Unauthorized access attempt to admin dashboard by user {Email}", 
                User.Identity?.Name);
            
            // Redirect non-admin users
            return RedirectToPage("/MyTasks");
        }

        var tasks = await _repository.GetAllTasksAsync();
        AllTasks = tasks.ToList();
        
        return Page();
    }
}
