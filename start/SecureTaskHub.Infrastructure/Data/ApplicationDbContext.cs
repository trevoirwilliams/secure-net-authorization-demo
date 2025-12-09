using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecureTaskHub.Domain.Entities;

namespace SecureTaskHub.Infrastructure.Data;

/// <summary>
/// Main database context for the SecureTaskHub application.
/// Extends IdentityDbContext to integrate ASP.NET Identity.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskItem> TaskItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure TaskItem entity
        builder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.OwnerUserId).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>();
            
            // TODO Secure: Add index on OwnerUserId for better query performance
            entity.HasIndex(e => e.OwnerUserId);
        });

        SeedData(builder);
    }

    private void SeedData(ModelBuilder builder)
    {
        // Create roles
        var adminRoleId = "07e5fcac-8993-438e-873f-8acfbce532dd";
        var userRoleId = "20fd6e8a-2b25-4efa-a109-a67f339d5402";

        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "2f4e9d57-8d6b-5c4f-0e3g-7b9d5c8f6e4b"

            },
            new IdentityRole
            {
                Id = userRoleId,
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = "1e3f8d46-7c5a-4b3e-9d2f-6a8c4b7e5d3a"

            }
        );

        // Create users
        // TODO Secure: These passwords are weak and only for demo purposes.
        // In production, use strong password policies and never seed with default passwords.
        
        var hasher = new PasswordHasher<IdentityUser>();

        var adminUser = new IdentityUser
        {
            Id = "d633d3fa-5890-413d-a0b7-242601a49a75",
            UserName = "admin@demo.local",
            NormalizedUserName = "ADMIN@DEMO.LOCAL",
            Email = "admin@demo.local",
            NormalizedEmail = "ADMIN@DEMO.LOCAL",
            EmailConfirmed = true,
            SecurityStamp = "38775744-6fe3-4f87-b8c8-ea3fcd7eb089",
            ConcurrencyStamp = "47287b02-ff20-4dc8-bf50-69da9d1a656a",
            PasswordHash = "AQAAAAIAAYagAAAAEJ3h9d5WYzthMCKEbVqfZYGmEP4rdZdKcLhKYo3EqXz+uGqC8X1Pq5SiJ4KdVkzSLQ=="

        };

        var aliceUser = new IdentityUser
        {
            Id = "495d56e4-43b1-4e54-9cef-10e917d28f63",
            UserName = "alice@demo.local",
            NormalizedUserName = "ALICE@DEMO.LOCAL",
            Email = "alice@demo.local",
            NormalizedEmail = "ALICE@DEMO.LOCAL",
            EmailConfirmed = true,
            SecurityStamp = "1a00fd73-7f7a-4719-9fd9-0e0d6a148efd",
            ConcurrencyStamp = "054ec42a-c1df-4b3f-ad55-f300ec6a853a",
            PasswordHash = "AQAAAAIAAYagAAAAEGxF7p4M3NqK8yWjLZv5rQwXzB9cT2sH6vN1kP8oY4eD3mR7aJ9iU5tL2fO6wE1xKg=="

        };

        var bobUser = new IdentityUser
        {
            Id = "21809ee3-a838-42e1-99b5-ad07b8deb31b",
            UserName = "bob@demo.local",
            NormalizedUserName = "BOB@DEMO.LOCAL",
            Email = "bob@demo.local",
            NormalizedEmail = "BOB@DEMO.LOCAL",
            EmailConfirmed = true,
            SecurityStamp = "4318e961-2a50-43fd-8f29-298a814a90fc",
            ConcurrencyStamp = "83ef9ad3-c995-479a-ac87-f41ef09796b6",
            PasswordHash = "AQAAAAIAAYagAAAAEDn8K5r2TvH6pM9wJ3sX1cY7oB4eP8qL2fN6kR9iZ3tA5mG1uV7hO4jE6wD8cS2nLx=="
        };

        builder.Entity<IdentityUser>().HasData(adminUser, aliceUser, bobUser);

        // Assign roles to users
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminUser.Id
            },
            new IdentityUserRole<string>
            {
                RoleId = userRoleId,
                UserId = aliceUser.Id
            },
            new IdentityUserRole<string>
            {
                RoleId = userRoleId,
                UserId = bobUser.Id
            }
        );

        // Seed task items
        builder.Entity<TaskItem>().HasData(
            // Admin's tasks
            new TaskItem
            {
                Id = 1,
                Title = "Review system security audit",
                Description = "Complete quarterly security audit review",
                Status = Domain.Enums.TaskStatus.InProgress,
                OwnerUserId = adminUser.Id,
                CreatedAt = new DateTime(2025, 11, 24, 0, 0, 0, DateTimeKind.Utc)
            },
            new TaskItem
            {
                Id = 2,
                Title = "Update admin dashboard",
                Description = "Add new reporting features to admin dashboard",
                Status = Domain.Enums.TaskStatus.New,
                OwnerUserId = adminUser.Id,
                CreatedAt = new DateTime(2025, 11, 26, 0, 0, 0, DateTimeKind.Utc)
            },
            // Alice's tasks
            new TaskItem
            {
                Id = 3,
                Title = "Complete project proposal",
                Description = "Write and submit Q1 project proposal",
                Status = Domain.Enums.TaskStatus.InProgress,
                OwnerUserId = aliceUser.Id,
                CreatedAt = new DateTime(2025, 11, 22, 0, 0, 0, DateTimeKind.Utc)
            },
            new TaskItem
            {
                Id = 4,
                Title = "Review code changes",
                Description = "Review pull requests from team members",
                Status = Domain.Enums.TaskStatus.New,
                OwnerUserId = aliceUser.Id,
                CreatedAt = new DateTime(2025, 11, 27, 0, 0, 0, DateTimeKind.Utc)
            },
            new TaskItem
            {
                Id = 5,
                Title = "Schedule team meeting",
                Description = "Organize monthly team sync",
                Status = Domain.Enums.TaskStatus.Done,
                OwnerUserId = aliceUser.Id,
                CreatedAt = new DateTime(2025, 11, 19, 0, 0, 0, DateTimeKind.Utc)
            },
            // Bob's tasks
            new TaskItem
            {
                Id = 6,
                Title = "Fix login bug",
                Description = "Debug and fix authentication issue reported by users",
                Status = Domain.Enums.TaskStatus.InProgress,
                OwnerUserId = bobUser.Id,
                CreatedAt = new DateTime(2025, 11, 25, 0, 0, 0, DateTimeKind.Utc)
            },
            new TaskItem
            {
                Id = 7,
                Title = "Write API documentation",
                Description = "Document new API endpoints for external developers",
                Status = Domain.Enums.TaskStatus.New,
                OwnerUserId = bobUser.Id,
                CreatedAt = new DateTime(2025, 11, 28, 0, 0, 0, DateTimeKind.Utc)
            },
            new TaskItem
            {
                Id = 8,
                Title = "Update dependencies",
                Description = "Update NuGet packages to latest stable versions",
                Status = Domain.Enums.TaskStatus.Done,
                OwnerUserId = bobUser.Id,
                CreatedAt = new DateTime(2025, 11, 14, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
