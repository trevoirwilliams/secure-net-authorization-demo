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
[Authorize(Roles = "Admin")]
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
        var tasks = await _repository.GetAllTasksAsync();
        AllTasks = tasks.ToList();
        
        return Page();
    }
}
