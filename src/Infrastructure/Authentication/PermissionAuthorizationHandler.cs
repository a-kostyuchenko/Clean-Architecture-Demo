using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authentication;

public class PermissionAuthorizationHandler 
    : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var permissions = context
            .User
            .Claims
            .Where(x => x.Type == CustomClaims.Permissions)
            .Select(x => x.Value)
            .ToHashSet();

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
        
        context.Fail();
        return Task.CompletedTask;
    }
}