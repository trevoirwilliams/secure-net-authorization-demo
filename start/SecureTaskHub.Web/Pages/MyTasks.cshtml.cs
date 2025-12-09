using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureTaskHub.Domain.Entities;
using SecureTaskHub.Domain.Interfaces;
using System.Security.Claims;

namespace SecureTaskHub.Web.Pages;

/// <summary>
/// Page that displays tasks for the current logged-in user.
/// Demonstrates proper horizontal access control.
/// </summary>
[Authorize]
public class MyTasksModel : PageModel
{
    private readonly ITaskItemRepository _repository;
    private readonly ILogger<MyTasksModel> _logger;

    public MyTasksModel(ITaskItemRepository repository, ILogger<MyTasksModel> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public List<TaskItem> Tasks { get; set; } = new();

    public async Task OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null)
        {
            var tasks = await _repository.GetTasksByUserIdAsync(userId);
            Tasks = tasks.ToList();
        }
    }
}
