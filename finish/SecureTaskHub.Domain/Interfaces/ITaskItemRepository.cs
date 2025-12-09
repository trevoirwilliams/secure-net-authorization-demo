using SecureTaskHub.Domain.Entities;

namespace SecureTaskHub.Domain.Interfaces;

/// <summary>
/// Repository interface for TaskItem operations.
/// Implementations should enforce horizontal access control (row-level security).
/// </summary>
public interface ITaskItemRepository
{
    /// <summary>
    /// Gets all tasks for a specific user.
    /// </summary>
    Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(string userId);

    /// <summary>
    /// Gets all tasks in the system (Admin only).
    /// </summary>
    Task<IEnumerable<TaskItem>> GetAllTasksAsync();

    /// <summary>
    /// Gets a task by ID if the user is authorized to view it.
    /// </summary>
    Task<TaskItem?> GetTaskByIdAsync(int id, string userId, bool isAdmin);

    /// <summary>
    /// VULNERABLE: Gets a task by ID WITHOUT any authorization checks.
    /// This method intentionally bypasses security for demonstration purposes.
    /// DO NOT use this in production code!
    /// </summary>
    Task<TaskItem?> GetTaskByIdWithoutAuthorizationAsync(int id);

    /// <summary>
    /// Creates a new task.
    /// </summary>
    Task<TaskItem> CreateTaskAsync(TaskItem task);

    /// <summary>
    /// Updates an existing task if the user is authorized.
    /// </summary>
    Task<bool> UpdateTaskAsync(TaskItem task, string userId, bool isAdmin);

    /// <summary>
    /// Deletes a task if the user is authorized.
    /// </summary>
    Task<bool> DeleteTaskAsync(int id, string userId, bool isAdmin);
}
