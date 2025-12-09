using Microsoft.AspNetCore.Authorization;

namespace SecureTaskHub.Web.Requirements
{
    public class TaskOwnerRequirement : IAuthorizationRequirement
    {
    }
}
