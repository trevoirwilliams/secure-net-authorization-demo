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
public class TaskDetailsModel : PageModel
{
    private readonly ITaskItemRepository _repository;
    private readonly ILogger<TaskDetailsModel> _logger;

    public TaskDetailsModel(ITaskItemRepository repository, ILogger<TaskDetailsModel> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public TaskItem? Task { get; set; }
    public int TaskId { get; set; }
    public bool IsVulnerabilityExposed { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        TaskId = id;
        
        // VULNERABILITY: Using the insecure method that bypasses authorization!
        // We're fetching the task by ID without verifying the current user owns it.
        Task = await _repository.GetTaskByIdWithoutAuthorizationAsync(id);

        if (Task == null)
        {
            return Page();
        }

        // Check if the user is viewing someone else's task
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId != null && Task.OwnerUserId != currentUserId)
        {
            IsVulnerabilityExposed = true;
            _logger.LogWarning(
                "SECURITY: User {CurrentUserId} ({Email}) accessed task {TaskId} owned by {OwnerId}",
                currentUserId,
                User.Identity?.Name,
                id,
                Task.OwnerUserId
            );
        }

        return Page();
    }
}
