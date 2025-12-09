using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureTaskHub.Api.Models;
using SecureTaskHub.Domain.Entities;
using SecureTaskHub.Domain.Interfaces;
using System.Security.Claims;

namespace SecureTaskHub.Api.Controllers;

/// <summary>
/// API controller for managing task items.
/// Demonstrates both secure and insecure endpoints for teaching purposes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication by default
public class TasksController : ControllerBase
{
    private readonly ITaskItemRepository _repository;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskItemRepository repository, ILogger<TasksController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Gets tasks for the current authenticated user.
    /// This endpoint properly enforces horizontal access control.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyTasks()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var tasks = await _repository.GetTasksByUserIdAsync(userId);
        var taskDtos = tasks.Select(MapToDto).ToList();
        
        return Ok(taskDtos);
    }

    /// <summary>
    /// INSECURE ENDPOINT - FOR TEACHING PURPOSES ONLY!
    /// TODO Secure: This endpoint returns ALL tasks without proper authorization checks.
    /// This is a deliberate vulnerability to demonstrate broken access control (OWASP A01:2021).
    /// 
    /// Problems:
    /// - Any authenticated user can see all tasks from all users
    /// - No role-based or ownership-based filtering
    /// - Violates horizontal access control principles
    /// 
    /// Fix: Either remove this endpoint or restrict it to Admin role only.
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllTasks()
    {
        // SECURITY GAP: No authorization check!
        _logger.LogWarning("INSECURE: GetAllTasks called by user {UserId}", User.FindFirstValue(ClaimTypes.NameIdentifier));
        
        var tasks = await _repository.GetAllTasksAsync();
        var taskDtos = tasks.Select(MapToDto).ToList();
        
        return Ok(taskDtos);
    }

    /// <summary>
    /// Gets tasks for admin users only (proper vertical access control).
    /// </summary>
    [HttpGet("admin/all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllTasksAdmin()
    {
        var tasks = await _repository.GetAllTasksAsync();
        var taskDtos = tasks.Select(MapToDto).ToList();
        
        return Ok(taskDtos);
    }

    /// <summary>
    /// Gets a specific task by ID.
    /// Enforces horizontal access control - users can only view their own tasks.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Admin");
        var task = await _repository.GetTaskByIdAsync(id, userId, isAdmin);

        if (task == null)
        {
            // TODO Secure: Consider whether to return 404 or 403
            // Returning 404 for both "not found" and "not authorized" can prevent information leakage
            return NotFound();
        }

        return Ok(MapToDto(task));
    }

    /// <summary>
    /// Creates a new task for the current user.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        // TODO Secure: Add input validation (FluentValidation, DataAnnotations, etc.)
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new { message = "Title is required" });
        }

        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            Status = Enum.Parse<Domain.Enums.TaskStatus>(request.Status, ignoreCase: true),
            OwnerUserId = userId, // Always set to current user
            CreatedAt = DateTime.UtcNow
        };

        var createdTask = await _repository.CreateTaskAsync(task);
        return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, MapToDto(createdTask));
    }

    /// <summary>
    /// Updates an existing task.
    /// Enforces horizontal access control - users can only update their own tasks.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        // TODO Secure: Add comprehensive input validation
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new { message = "Title is required" });
        }

        var isAdmin = User.IsInRole("Admin");
        
        // TODO Secure: This has a potential IDOR vulnerability if OwnerUserId can be modified
        var task = new TaskItem
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            Status = Enum.Parse<Domain.Enums.TaskStatus>(request.Status, ignoreCase: true),
            OwnerUserId = userId // Ensure we don't allow ownership transfer without proper checks
        };

        var success = await _repository.UpdateTaskAsync(task, userId, isAdmin);
        
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a task.
    /// Enforces horizontal access control - users can only delete their own tasks.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Admin");
        var success = await _repository.DeleteTaskAsync(id, userId, isAdmin);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    private static TaskItemDto MapToDto(TaskItem task)
    {
        return new TaskItemDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            OwnerUserId = task.OwnerUserId,
            CreatedAt = task.CreatedAt
        };
    }
}
