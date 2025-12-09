using Microsoft.EntityFrameworkCore;
using SecureTaskHub.Domain.Entities;
using SecureTaskHub.Domain.Interfaces;
using SecureTaskHub.Infrastructure.Data;

namespace SecureTaskHub.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for TaskItem with horizontal access control.
/// Ensures users can only access their own tasks unless they are admins.
/// </summary>
public class TaskItemRepository : ITaskItemRepository
{
    private readonly ApplicationDbContext _context;

    public TaskItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(string userId)
    {
        // TODO Secure: Consider adding pagination to prevent large data exposure
        return await _context.TaskItems
            .Where(t => t.OwnerUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
    {
        // TODO Secure: This should only be called by admin users
        // The authorization check should happen in the controller/API layer
        return await _context.TaskItems
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id, string userId, bool isAdmin)
    {
        var task = await _context.TaskItems.FindAsync(id);
        
        if (task == null)
            return null;

        // Horizontal access control: users can only see their own tasks
        if (!isAdmin && task.OwnerUserId != userId)
        {
            // TODO Secure: Consider logging unauthorized access attempts
            return null;
        }

        return task;
    }

    /// <summary>
    /// VULNERABLE: Gets a task by ID WITHOUT any authorization checks.
    /// This method intentionally bypasses security for demonstration purposes.
    /// This is an example of INSECURE code that demonstrates IDOR vulnerability.
    /// </summary>
    public async Task<TaskItem?> GetTaskByIdWithoutAuthorizationAsync(int id)
    {
        // VULNERABILITY: No authorization check whatsoever!
        // Any authenticated user can retrieve any task by ID
        return await _context.TaskItems.FindAsync(id);
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem task)
    {
        // TODO Secure: Validate that OwnerUserId matches the authenticated user
        // or that the user is an admin creating on behalf of someone
        _context.TaskItems.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> UpdateTaskAsync(TaskItem task, string userId, bool isAdmin)
    {
        var existingTask = await _context.TaskItems.FindAsync(task.Id);
        
        if (existingTask == null)
            return false;

        // Horizontal access control: users can only update their own tasks
        if (!isAdmin && existingTask.OwnerUserId != userId)
        {
            // TODO Secure: Log unauthorized update attempts
            return false;
        }

        // TODO Secure: Prevent users from changing OwnerUserId
        // Currently this allows ownership transfer which could be a security issue
        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.Status = task.Status;
        //existingTask.OwnerUserId = task.OwnerUserId; // SECURITY GAP: Should validate this!

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTaskAsync(int id, string userId, bool isAdmin)
    {
        var task = await _context.TaskItems.FindAsync(id);
        
        if (task == null)
            return false;

        // Horizontal access control: users can only delete their own tasks
        if (!isAdmin && task.OwnerUserId != userId)
        {
            // TODO Secure: Log unauthorized delete attempts
            return false;
        }

        _context.TaskItems.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }
}
