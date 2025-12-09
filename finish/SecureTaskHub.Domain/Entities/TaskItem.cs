using System.ComponentModel.DataAnnotations;

namespace SecureTaskHub.Domain.Entities;

/// <summary>
/// Represents a task item in the system.
/// Each task belongs to a specific user (OwnerUserId).
/// </summary>
public class TaskItem
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.New;

    /// <summary>
    /// The user ID of the owner of this task.
    /// This is used for horizontal access control - users should only see their own tasks.
    /// </summary>
    [Required]
    public string OwnerUserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
