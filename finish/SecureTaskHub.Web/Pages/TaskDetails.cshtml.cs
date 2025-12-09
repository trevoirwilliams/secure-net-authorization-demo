using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureTaskHub.Domain.Entities;
using SecureTaskHub.Domain.Interfaces;
using System.Security.Claims;

namespace SecureTaskHub.Web.Pages;

/// <summary>
/// Page that displays details of a single task.
/// 
/// SECURITY VULNERABILITY: Insecure Direct Object Reference (IDOR)
/// This page does NOT verify that the task belongs to the current user.
/// Users can change the ID in the URL to view other users' tasks.
/// 
/// Example: If Alice is logged in and Bob's task has ID=5, Alice can navigate to
/// /TaskDetails/5 and see Bob's task, even though she shouldn't have access to it.
/// 
/// This demonstrates OWASP Top 10 A01:2021 - Broken Access Control
/// </summary>
[Authorize]
public class TaskDetailsModel(ITaskItemRepository repository, ILogger<TaskDetailsModel> logger,
    IAuthorizationService authorizationService) : PageModel
{
    public TaskItem? Task { get; set; }
    public int TaskId { get; set; }
    public bool IsVulnerabilityExposed { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        TaskId = id;

        var task = await repository.GetTaskByIdWithoutAuthorizationAsync(id);
        if (task == null) return NotFound();

        var authResult = await authorizationService.AuthorizeAsync(User, task, "TaskOwnerPolicy");
        if (!authResult.Succeeded) return Forbid();

        Task = task;

        return Page();
    }
}
