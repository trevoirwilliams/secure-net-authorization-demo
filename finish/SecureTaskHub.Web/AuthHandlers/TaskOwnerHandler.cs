using Microsoft.AspNetCore.Authorization;
using SecureTaskHub.Domain.Entities;
using SecureTaskHub.Infrastructure.Data;
using SecureTaskHub.Web.Requirements;
using System.Security.Claims;

namespace SecureTaskHub.Web.AuthHandlers
{
    public class TaskOwnerHandler : AuthorizationHandler<TaskOwnerRequirement, Domain.Entities.TaskItem>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            TaskOwnerRequirement requirement, 
            TaskItem resource)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null && resource != null && resource.OwnerUserId == userId)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
